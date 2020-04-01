using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Services
{
    public interface IServiceProviderResolver
    {
        IServiceProvider GetProvider();
    }

    public interface IScopedServiceProviderResolver : IServiceProviderResolver
    {
        /// <summary>
        /// Get the service provider for scoped services
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetProvider();
    }

    public class ScopedServiceProvider : IScopedServiceProviderResolver
    {
        IServiceProvider _service;

        public ScopedServiceProvider(IServiceProvider service)
        {
            _service = service;
        }

        public IServiceProvider GetProvider() => _service;
        
    }

}
