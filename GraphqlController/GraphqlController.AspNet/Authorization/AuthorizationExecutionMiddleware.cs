using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Instrumentation;

namespace GraphqlController.AspNetCore.Authorization
{
    public class AuthorizationExecutionMiddleware : IExecutionMiddleware
    {
        public async Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next)
        {
            executionContext.FieldMiddleware.Use<AuthorizationFieldMiddleware>();
            await next();
        }
    }
}
