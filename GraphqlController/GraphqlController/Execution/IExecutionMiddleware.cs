using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public interface IExecutionMiddleware
    {
        public Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next);
    }
}
