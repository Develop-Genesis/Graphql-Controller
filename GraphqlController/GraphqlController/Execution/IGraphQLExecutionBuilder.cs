using GraphQL.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public interface IGraphQLExecutionBuilder
    {
        IGraphqlExecution BuildExecution(
            GraphQlRequest request, 
            DocumentExecuterMidleware documentExecuterMidleware, 
            IExecutionDataDictionary data,
            IServiceProvider serviceProvider, 
            CancellationToken cancellationToken);

        IGraphQLExecutionBuilder Use<T>() where T : IExecutionMiddleware;
        IGraphQLExecutionBuilder Use(Func<ExecutionContext, Func<Task>, Task> middlewareDelegate);
    }
}
