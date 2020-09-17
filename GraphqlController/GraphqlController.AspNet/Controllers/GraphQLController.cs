using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Instrumentation;
using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphqlController.AspNetCore
{
    public class GraphQLController : ControllerBase
    {
        ISchemaRouteService _schemaRouteService;
        IGraphQLExecutor _executor;
        IExecutionBuilderResolver _executionBuilderResolver;

        public GraphQLController(
            IGraphQLExecutor executor,
            ISchemaRouteService schemaRouteService,            
            IExecutionBuilderResolver executionBuilderResolver)
        {
            _executor = executor;
            _schemaRouteService = schemaRouteService;
            _executionBuilderResolver = executionBuilderResolver;
        }
                
        [HttpPost]
        public async Task<ActionResult<Dictionary<string, object>>> ExecuteQuery(CancellationToken cancellationToken)
        {
            string bodyText;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                bodyText = await reader.ReadToEndAsync();
            }

            var request = JsonConvert.DeserializeObject<GraphQlRequest>(bodyText);

            var result = await ExecuteAsync(request, cancellationToken);

            if (HttpContext.Response.StatusCode != 200)
            {
                return StatusCode(HttpContext.Response.StatusCode, result.result);
            }

            return new ActionResult<Dictionary<string, object>>(result.result);
        }

        [HttpGet]
        public async Task<ActionResult<Dictionary<string, object>>> ExecuteQuery([FromQuery]string query, [FromQuery]string operationName, [FromQuery]string variables, [FromQuery]string extensions, CancellationToken cancellationToken)
        {
            var request = new GraphQlRequest()
            {
                Query = query,
                OperationName = operationName,
                Variables = variables != null ? JsonConvert.DeserializeObject<JObject>(variables) : null,
                Extensions = extensions != null ? JsonConvert.DeserializeObject<JObject>(extensions) : null
            };

            var result = await ExecuteAsync(request, cancellationToken);

            if(HttpContext.Response.StatusCode != 200)
            {
                return StatusCode(HttpContext.Response.StatusCode);
            }

            return new ActionResult<Dictionary<string, object>>(result.result);
        }

        async Task<(Dictionary<string, object> result, IExecutionDataDictionary data)> ExecuteAsync(GraphQlRequest graphQlRequest, CancellationToken cancellationToken)
        {
            var root = GetRoot();
            var executionBuilder = _executionBuilderResolver.GetGraphqlExecutionBuilder(root);

            var result = await _executor.ExecuteAsync(executionBuilder, graphQlRequest, root, new ExecutionDataDictionary() {
                { "IsHttpRequest", true },
                { "HttpContext", HttpContext }
            }, cancellationToken);

            var dictionary = result.ExecutionResult.ToResultDictionary();

            return (dictionary, result.ExecutionData);
        }

        System.Type GetRoot()
        {
            return _schemaRouteService.GetType(ControllerContext.HttpContext.Request.Path.Value);            
        }

    }
}
