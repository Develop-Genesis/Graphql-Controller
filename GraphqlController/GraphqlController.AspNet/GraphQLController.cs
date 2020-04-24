using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphqlController.AspNetCore;
using GraphqlController.Execution;
using GraphqlController.Services;
using Microsoft.AspNetCore.Mvc;


namespace GraphqlController.AspNetCore
{   
    public class GraphQLController : ControllerBase
    {
        ISchemaRouteService _schemaRouteService;
        IGraphQLExecutor _executor;

        public GraphQLController(
            IGraphQLExecutor executor,
            ISchemaRouteService schemaRouteService)
        {            
            _executor = executor;
            _schemaRouteService = schemaRouteService;
        }
                
        [HttpPost]
        public Task<GraphQlResponse> ExecuteQuery([FromBody]GraphQlRequestBody body, CancellationToken cancellationToken)
            => Execute(body.Query, body.OperationName, null/*body.Variables?.ToInputs()*/, cancellationToken);

        [HttpGet]
        public Task<GraphQlResponse> ExecuteQuery([FromQuery]string query, [FromQuery]string operationName, [FromQuery]string variables, CancellationToken cancellationToken)
           => Execute(query, operationName, variables?.ToInputs(), cancellationToken);

        async Task<GraphQlResponse> Execute(string query, string operationName, Inputs variables, CancellationToken cancellationToken)
        {
            var root = GetRoot();

            var result = await _executor.ExecuteAsync(_ =>
            {
                _.Query = query;
                _.OperationName = operationName;
                _.Inputs = variables;
                _.CancellationToken = cancellationToken;
            }, root);

            return new GraphQlResponse()
            {
                Data = result.Data,
                Errors = result.Errors?.Select(error => new GraphQLError()
                {
                    Message = error.Message,
                    Path = error.Path,
                    Locations = error.Locations?.Select(loc => new ErrorLocation
                    {
                        Column = loc.Column,
                        Line = loc.Line
                    }),
                    Extensions = error.DataAsDictionary
                })
            };
        }

        Type GetRoot()
        {
            return _schemaRouteService.GetType(ControllerContext.HttpContext.Request.Path.Value);            
        }

    }
}
