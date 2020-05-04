
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public class DocumentExecuterMidleware : IExecutionMiddleware
    {
        ISchema _schema;
        IServiceProvider _serviceProvider;

        public DocumentExecuterMidleware(ISchema schema, IServiceProvider serviceProvider)
        {
            _schema = schema;
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(ExecutionContext executionContext, Func<Task> next)
        {
            var documentExecuter = new DocumentExecuter();

            executionContext.UserContext.Add("serviceProvider", _serviceProvider);

            var result = await documentExecuter.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = executionContext.Request.Query;
                _.OperationName = executionContext.Request.OperationName;
                _.Inputs = executionContext.Request.Variables?.ToInputs();
                _.CancellationToken = executionContext.CancellationToken;
                _.ComplexityConfiguration = executionContext.ComplexityConfiguration;
                _.EnableMetrics = executionContext.EnableMetrics;
                _.ExposeExceptions = executionContext.ExposeExceptions;
                _.FieldMiddleware = executionContext.FieldMiddleware;
                
                foreach(var listener in executionContext.Listeners)
                {
                    _.Listeners.Add(listener);
                }

                _.MaxParallelExecutionCount = executionContext.MaxParallelExecutionCount;
                _.ThrowOnUnhandledException = executionContext.ThrowOnUnhandledException;
                _.UnhandledExceptionDelegate = executionContext.UnhandledExceptionDelegate;
                _.UserContext = executionContext.UserContext;
                _.ValidationRules = executionContext.ValidationRules.Concat(DocumentValidator.CoreRules);
                _.UserContext = executionContext.UserContext;                
            });

            executionContext.Result = result;
        }
    }
}
