using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Instrumentation;
using GraphqlController.AspNetCore;
using GraphqlController.AspNetCore.Cache;
using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
using GraphqlController.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphqlController.AspNetCore
{   
    public class GraphQLController : ControllerBase
    {
        ISchemaRouteService _schemaRouteService;
        IGraphQLExecutor _executor;
        IPersistedQueryService _persistedQueryService;
        IFieldMiddlewareBuilder _fieldMiddlewareBuilder;

        IGraphqlInAppCacheService _cacheInAppService;
        ICachePolicy _cachePolicy;
        CacheConfiguration _cacheConfiguration;

        public GraphQLController(
            IGraphqlInAppCacheService cacheInAppService,
            IGraphQLExecutor executor,
            ISchemaRouteService schemaRouteService,
            IPersistedQueryService persistedQueryService,
            IFieldMiddlewareBuilder fieldMiddlewareBuilder,
            ICachePolicy cachePolicy,
            CacheConfiguration cacheConfiguration)
        {
            _cacheConfiguration = cacheConfiguration;
            _executor = executor;
            _schemaRouteService = schemaRouteService;
            _persistedQueryService = persistedQueryService;
            _fieldMiddlewareBuilder = fieldMiddlewareBuilder;
            _cacheInAppService = cacheInAppService;
            _cachePolicy = cachePolicy;
        }
                
        [HttpPost]
        public async Task<Dictionary<string, object>> ExecuteQuery(CancellationToken cancellationToken)
        {
            string bodyText;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                bodyText = await reader.ReadToEndAsync();
            }

            var request = JsonConvert.DeserializeObject<GraphQlRequest>(bodyText);

            return await ExecuteAndCheckPersistedQuery(request, cancellationToken);
        }              

        [HttpGet]
        public async Task<ActionResult<Dictionary<string, object>>> ExecuteQuery([FromQuery]string query, [FromQuery]string operationName, [FromQuery]string variables, [FromQuery]string extensions, CancellationToken cancellationToken)
        {
            var request = new GraphQlRequest()
            {
                Query = query,
                OperationName = operationName,
                Variables = variables != null ? JsonConvert.DeserializeObject<JObject>(variables) : null,
                Extensions = extensions != null ? JsonConvert.DeserializeObject<GraphqlExtensions>(extensions) : null
            };

            var result = await ExecuteAndCheckPersistedQuery(request, cancellationToken);

            if(_cacheConfiguration.UseHttpCaching)
            {
                var maxAge = _cachePolicy.CalculateMaxAge();
                var scope = _cachePolicy.GetScope();
                if(maxAge == 0)
                {
                    HttpContext.Response.Headers.Add(HeaderNames.CacheControl, "no-cache");
                }
                else
                {
                    HttpContext.Response.Headers.Add(HeaderNames.CacheControl, $"{scope.ToHttpHeader()}, max-age={maxAge}");
                }                
            }

            if (_cacheConfiguration.IncludeETag)
            {
                var etag = Helpers.GetSha256Hash(JsonConvert.SerializeObject(result));

                // Check if the client has if not match header with etag
                StringValues clientIfNotMatch;
                if (HttpContext.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out clientIfNotMatch))
                {
                    if (clientIfNotMatch.FirstOrDefault() == etag)
                    {
                        // send 304 status code (Not modified)
                        return StatusCode(304);
                    }
                }

                HttpContext.Response.Headers.Add(HeaderNames.ETag, etag);
            }

            return new ActionResult<Dictionary<string, object>>(result);
        }


        public async Task<Dictionary<string, object>> ExecuteAndCheckPersistedQuery(GraphQlRequest request, CancellationToken cancellationToken)
        {
            var result = await _persistedQueryService.TryRegisterQueryAsync(request, cancellationToken) switch
            {
                RegisterPersistedQueryResult.QueryRegistered => await Execute(request.Query, request.OperationName, request.Variables?.ToInputs(), cancellationToken),
                RegisterPersistedQueryResult.NoQueryToRegister => await ExecutePersistedQuery(request, cancellationToken),
                RegisterPersistedQueryResult.PersistedQueryNotSupported => new Dictionary<string, object> { { "errors", new GraphQLError[] { new GraphQLError() { Message = "PersistedQueryNotSupported" } } } },
                RegisterPersistedQueryResult.NoPersistedQuery => await Execute(request.Query, request.OperationName, request.Variables?.ToInputs(), cancellationToken),
                _ => throw new InvalidOperationException(),
            };

            return result;
        }

        public async Task<Dictionary<string, object>> ExecutePersistedQuery(GraphQlRequest request, CancellationToken cancellationToken)
        {
            var query = await _persistedQueryService.GetPersistedQueryAsync(request.Extensions.PersistedQuery.Sha256Hash, cancellationToken);

            if(query == null)
            {
                return new Dictionary<string, object> { { "errors", new GraphQLError[] { new GraphQLError() { Message = "PersistedQueryNotFound" } } } };
            }

            return await Execute(query, request.OperationName, request.Variables?.ToInputs(), cancellationToken);
        }

        async Task<Dictionary<string, object>> Execute(string query, string operationName, Inputs variables, CancellationToken cancellationToken)
        {
            var root = GetRoot();

            var cachedResponse = await _cacheInAppService.GetCachedResponseAsync(query, operationName, variables, cancellationToken);

            if(cachedResponse != null)
            {
                return new Dictionary<string, object>() 
                {
                    {"data", cachedResponse }
                };
            }

            var result = await _executor.ExecuteAsync(_ =>
            {                
                _.Query = query;
                _.OperationName = operationName;
                _.Inputs = variables;
                _.CancellationToken = cancellationToken;
                _.FieldMiddleware = _fieldMiddlewareBuilder;
            }, root);

            var dictionary = new Dictionary<string, object>();

            dictionary["data"] = result.Data;
            dictionary["errors"] = result.Errors?.Select(error => new GraphQLError()
               {
                Message = error.Message,
                Path = error.Path,
                Locations = error.Locations?.Select(loc => new ErrorLocation
                {
                    Column = loc.Column,
                    Line = loc.Line
                }),
                Extensions = error.DataAsDictionary
            });

            if(result.Errors == null)
            {
                await _cacheInAppService.CacheResponseAsync(query, operationName, variables, result.Data, cancellationToken);
            }

            return dictionary;
        }

        System.Type GetRoot()
        {
            return _schemaRouteService.GetType(ControllerContext.HttpContext.Request.Path.Value);            
        }

    }
}
