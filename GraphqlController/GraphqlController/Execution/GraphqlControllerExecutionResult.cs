using GraphQL;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Execution
{
    public class GraphqlControllerExecutionResult
    {
        public GraphqlControllerExecutionResult(ExecutionResult executionResult, IExecutionDataDictionary executionData)
        {
            ExecutionResult = executionResult ?? throw new ArgumentNullException(nameof(executionResult));
            ExecutionData = executionData ?? throw new ArgumentNullException(nameof(executionData));
        }

        public ExecutionResult ExecutionResult { get; }
        public IExecutionDataDictionary ExecutionData { get; }
    }
}
