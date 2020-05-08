using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class ExecutionPersistedQueryMiddleware : IExecutionMiddleware
    {
        IPersistedQueryService _persistedQueryService;

        public ExecutionPersistedQueryMiddleware(IPersistedQueryService persistedQueryService)
        {
            _persistedQueryService = persistedQueryService;
        }

        public async Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next)
        {

            switch (await _persistedQueryService.TryRegisterQueryAsync(executionContext.Request, executionContext.CancellationToken))
            {
                case RegisterPersistedQueryResult.PersistedQueryNotSupported:
                    executionContext.Result.Errors = new GraphQL.ExecutionErrors()
                    {
                        new GraphQL.ExecutionError("PersistedQueryNotSupported")
                    };
                    return;

                case RegisterPersistedQueryResult.NoQueryToRegister:
                    var query = await _persistedQueryService.GetPersistedQueryAsync(
                        (string)executionContext.Request.Extensions["persistedQuery"]["sha256Hash"], executionContext.CancellationToken);

                    if (query == null)
                    {
                        executionContext.Result = new GraphQL.ExecutionResult()
                        {
                            Errors = new GraphQL.ExecutionErrors()
                            {
                                new GraphQL.ExecutionError("PersistedQueryNotFound")
                            }
                        };
                        return;
                    }

                    executionContext.Request.Query = query;
                    break;
            }

            await next();

        }
    }
}
