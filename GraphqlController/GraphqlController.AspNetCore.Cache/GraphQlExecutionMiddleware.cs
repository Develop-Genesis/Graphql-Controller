using GraphqlController.Execution;
using GraphQL.Instrumentation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using System.Linq;
using GraphQL;

namespace GraphqlController.AspNetCore.Cache
{
    public class GraphQlExecutionMiddleware : IExecutionMiddleware
    {
        IGraphqlInAppCacheService _cacheInAppService;
        ICachePolicy _cachePolicy;
        CacheConfiguration _cacheConfiguration;

        public GraphQlExecutionMiddleware(
            IGraphqlInAppCacheService cacheInAppService,
            ICachePolicy cachePolicy,
            CacheConfiguration cacheConfiguration)
        {
            _cacheConfiguration = cacheConfiguration;
            _cachePolicy = cachePolicy;
            _cacheConfiguration = cacheConfiguration;
            _cacheInAppService = cacheInAppService;
        }


        public async Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next)
        {
            // check if the request is already cached
            var cachedResponse = await _cacheInAppService.GetCachedResponseAsync(
                executionContext.Request.Query,
                executionContext.Request.OperationName,
                executionContext.Request.Variables.ToInputs(),
                executionContext.CancellationToken);

            // If the response is already cached dont execute the next middleware
            if (cachedResponse != null)
            {
                executionContext.Result = new ExecutionResult();
                executionContext.Result.Data = cachedResponse;
            }
            else
            {
                // register the cache field middleware, pre execution
                executionContext.FieldMiddleware.Use<CacheFieldMiddleware>();
                await next();
            }

            if (executionContext.Result.Errors == null)
            {
                await _cacheInAppService.CacheResponseAsync(
                    executionContext.Request.Query, 
                    executionContext.Request.OperationName,
                    executionContext.Request.Variables.ToInputs(),
                    executionContext.Result.Data,
                    executionContext.CancellationToken);
            }

            if (!executionContext.ExecutionData.IsHttpRequest())
            {
                return;
            }

            var httpContext = executionContext.ExecutionData.GetHttpContext();
            if (_cacheConfiguration.UseHttpCaching)
            {
                var maxAge = _cachePolicy.CalculateMaxAge();
                var scope = _cachePolicy.GetScope();
                if (maxAge == 0)
                {
                    httpContext.Response.Headers.Add(HeaderNames.CacheControl, "no-cache");
                }
                else
                {
                    httpContext.Response.Headers.Add(HeaderNames.CacheControl, $"{scope.ToHttpHeader()}, max-age={maxAge}");
                }
            }

            if (_cacheConfiguration.IncludeETag)
            {
                var etag = Helpers.GetSha256Hash(JsonConvert.SerializeObject(executionContext.Result.Data));

                // Check if the client has if not match header with etag
                StringValues clientIfNotMatch;
                if (httpContext.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out clientIfNotMatch))
                {
                    if (clientIfNotMatch.FirstOrDefault() == etag)
                    {
                        // send 304 status code (Not modified)
                        httpContext.Response.StatusCode = 304;
                    }
                }

                httpContext.Response.Headers.Add(HeaderNames.ETag, etag);
            }
        }
    }
}
