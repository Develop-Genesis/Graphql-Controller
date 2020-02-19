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
