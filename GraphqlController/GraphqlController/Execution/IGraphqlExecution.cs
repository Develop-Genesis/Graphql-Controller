using GraphQL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public interface IGraphqlExecution
    {
        Task<ExecutionResult> ExecuteAsync();
    }
}
