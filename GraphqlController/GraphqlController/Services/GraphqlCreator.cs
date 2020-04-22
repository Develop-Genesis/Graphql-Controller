using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Services
{
    public class GraphqlResolver : IGraphqlResolver
    {
        IServiceProvider _serviceProvider; 

        public GraphqlResolver(IScopedServiceProviderResolver serviceProviderResolver)
        {
            _serviceProvider = serviceProviderResolver.GetProvider();
        }

        public Task<T> CreateGraphqlEnityAsync<T>(CancellationToken cancellationToken = default) where T : GraphNodeType
            => CreateGraphqlEnityAsync<T, object>(null, cancellationToken);
        

        public async Task<T> CreateGraphqlEnityAsync<T, P>(P parameters, CancellationToken cancellationToken = default) where T : GraphNodeType<P>
        {
            var instance = _serviceProvider.GetService(typeof(T)) as GraphNodeType<P>;
            await instance.OnCreateAsync(parameters, cancellationToken);
            return (T)instance;
        }
    }
}
