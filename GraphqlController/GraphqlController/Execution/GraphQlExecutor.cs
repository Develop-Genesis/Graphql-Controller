using GraphQL;
using GraphqlController.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Execution
{
    public class GraphQlExecutor : IGraphQLExecutor
    {
        ISchemaResolver _schemaResolver;
        IScopedServiceProviderResolver _scopedServiceProviderResolver;

        public GraphQlExecutor(ISchemaResolver schemaResolver, IScopedServiceProviderResolver scopedServiceProviderResolver)
        {
            _schemaResolver = schemaResolver;
            _scopedServiceProviderResolver = scopedServiceProviderResolver;
        }

        public Task<ExecutionResult> ExecuteAsync(Action<ExecutionOptions> configure, Type rootType)
        {
            var schema = _schemaResolver.GetSchema(rootType);
            var executor = new DocumentExecuter();

            return executor.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.UserContext = new Dictionary<string, object>() { { "serviceProvider", _scopedServiceProviderResolver.GetProvider() } };                
                configure(_);
            });
        }

        public Task<GraphqlControllerExecutionResult> 
            ExecuteAsync(IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, Type rootType, ExecutionDataDictionary data, CancellationToken cancellationToken)
        {
            var schema = _schemaResolver.GetSchema(rootType);

            var serviceProvider = _scopedServiceProviderResolver.GetProvider();

            var documentExecuterMiddleware = new DocumentExecuterMidleware(schema, serviceProvider);

            var execution = executionBuilder.BuildExecution(request, documentExecuterMiddleware, data, serviceProvider, cancellationToken);

            return execution.ExecuteAsync();
        }

        public Task<GraphqlControllerExecutionResult> ExecuteAsync<T>
            (IGraphQLExecutionBuilder executionBuilder, GraphQlRequest request, ExecutionDataDictionary data, CancellationToken cancellationToken)
           => ExecuteAsync(executionBuilder, request, typeof(T), data, cancellationToken);
    }
}
