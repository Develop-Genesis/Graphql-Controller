using GraphQL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public interface IGraphQLExecutor
    {
        Task<ExecutionResult> ExecuteAsync(Action<ExecutionOptions> configure, Type rootType);
        Task<ExecutionResult> ExecuteAsync<T>(Action<ExecutionOptions> configure);
    }
}
