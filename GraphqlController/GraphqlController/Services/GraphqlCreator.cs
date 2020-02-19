using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Services
{
    public class GraphqlCreator : IGraphqlCreator
    {
        IServiceProvider _serviceProvider; 

        public GraphqlCreator(IScopedServiceProviderResolver serviceProviderResolver)
        {
            _serviceProvider = serviceProviderResolver.GetProvider();
        }

        public async Task<T> CreateGraphqlEnityAsync<T, P>(P parameters) where T : GraphNodeType<P>
        {
            var instance = _serviceProvider.GetService(typeof(T)) as GraphNodeType<P>;
            await instance.OnCreateAsync(parameters);
            return (T)instance;
        }
    }
}
