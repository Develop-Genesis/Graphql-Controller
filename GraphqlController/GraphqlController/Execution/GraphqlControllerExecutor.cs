using GraphQL;
using GraphqlController.Services;
using System;
using System.Collections.Generic;
using System.Text;
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

        public Task<ExecutionResult> ExecuteAsync<T>(Action<ExecutionOptions> configure)
          => ExecuteAsync(configure, typeof(T));
    }
}
