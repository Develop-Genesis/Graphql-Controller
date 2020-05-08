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
        Task<GraphqlControllerExecutionResult> ExecuteAsync(IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, Type rootType, ExecutionDataDictionary data, CancellationToken cancellationToken);
        Task<GraphqlControllerExecutionResult> ExecuteAsync<T>(IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, ExecutionDataDictionary data, CancellationToken cancellationToken);
    }
}
