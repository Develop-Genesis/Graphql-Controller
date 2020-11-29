using GraphqlController.Attributes;
using GraphqlController.Helpers;
using GraphqlController.Services;
using GraphqlController.AspNetCore.Relay.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Relay
{
    public interface IObjectResolverService
    {
        Task<INode> GetByIdAsync(GlobalId id, CancellationToken cancellationToken);
    }

    public class ObjectResolverService : IObjectResolverService
    {
        ObjectResolversContianer _objectResolversContianer;
        IServiceProvider _serviceProvider;

        public ObjectResolverService(ObjectResolversContianer objectResolversContianer, IScopedServiceProviderResolver serviceProvider)
        {
            _objectResolversContianer = objectResolversContianer;
            _serviceProvider = serviceProvider.GetProvider();
        }

        public Task<INode> GetByIdAsync(GlobalId id, CancellationToken cancellationToken)
        {
            var resolverType = _objectResolversContianer.GetResolver(id.ObjectType);
            var resolver = _serviceProvider.GetService(resolverType) as INodeResolver;
            return resolver.GetByIdAsync(id, cancellationToken);
        }
    }
}
