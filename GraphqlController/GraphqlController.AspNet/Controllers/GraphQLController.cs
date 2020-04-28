using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphqlController.AspNetCore;
using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
using GraphqlController.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphqlController.AspNetCore
{   
    public class GraphQLController : ControllerBase
    {
        ISchemaRouteService _schemaRouteService;
        IGraphQLExecutor _executor;
        IPersistedQueryService _persistedQueryService;

        public GraphQLController(
            IGraphQLExecutor executor,
            ISchemaRouteService schemaRouteService,
            IPersistedQueryService persistedQueryService)
        {            
            _executor = executor;
            _schemaRouteService = schemaRouteService;
            _persistedQueryService = persistedQueryService;
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
        public async Task<Dictionary<string, object>> ExecuteQuery([FromQuery]string query, [FromQuery]string operationName, [FromQuery]string variables, [FromQuery]string extensions, CancellationToken cancellationToken)
        {
            var request = new GraphQlRequest()
            {
                Query = query,
                OperationName = operationName,
                Variables = variables != null ? JsonConvert.DeserializeObject<JObject>(variables) : null,
                Extensions = extensions != null ? JsonConvert.DeserializeObject<GraphqlExtensions>(extensions) : null
            };

            var result = await ExecuteAndCheckPersistedQuery(request, cancellationToken);
            return result;
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

            var result = await _executor.ExecuteAsync(_ =>
            {                
                _.Query = query;
                _.OperationName = operationName;
                _.Inputs = variables;
                _.CancellationToken = cancellationToken;
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

            return dictionary;
        }

        Type GetRoot()
        {
            return _schemaRouteService.GetType(ControllerContext.HttpContext.Request.Path.Value);            
        }

    }
}
