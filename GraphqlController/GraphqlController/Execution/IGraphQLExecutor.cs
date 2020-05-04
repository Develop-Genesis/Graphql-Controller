using GraphQL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public interface IGraphQLExecutor
    {
        Task<ExecutionResult> ExecuteAsync(IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, Type rootType, CancellationToken cancellationToken);
        Task<ExecutionResult> ExecuteAsync<T>(IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, CancellationToken cancellationToken);
    }
}
