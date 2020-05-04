using GraphQL;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GraphqlController.Execution
{
    public class ExecutionContext
    {
        public ExecutionContext(GraphQlRequest request, CancellationToken cancellationToken)
        {
            Request = request;
            Result = null;
            FieldMiddleware = new FieldMiddlewareBuilder();
            ValidationRules = new List<IValidationRule>();
            CancellationToken = cancellationToken;
            UserContext = new Dictionary<string, object>();
            Listeners = new List<IDocumentExecutionListener>();
            ComplexityConfiguration = new ComplexityConfiguration();
            EnableMetrics = false;
            ExposeExceptions = false;
            MaxParallelExecutionCount = null;
            ThrowOnUnhandledException = false;
            UnhandledExceptionDelegate = null;
        }

        public GraphQlRequest Request { get; }

        public ExecutionResult Result { get; set; }

        public IFieldMiddlewareBuilder FieldMiddleware { get; }

        public IList<IValidationRule> ValidationRules { get; }

        public CancellationToken CancellationToken { get; }

        public IDictionary<string, object> UserContext { get; }

        public IList<IDocumentExecutionListener> Listeners { get; }

        public ComplexityConfiguration ComplexityConfiguration { get; }

        public bool EnableMetrics { get; set; }

        public bool ExposeExceptions { get; set; }

        public int? MaxParallelExecutionCount { get; set; }

        public bool ThrowOnUnhandledException { get; set; }

        public Action<UnhandledExceptionContext> UnhandledExceptionDelegate { get; set; }
    }
}
