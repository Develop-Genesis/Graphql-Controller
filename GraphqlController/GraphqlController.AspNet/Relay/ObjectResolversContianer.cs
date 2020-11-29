using GraphqlController.Attributes;
using GraphqlController.Helpers;
using GraphqlController.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.AspNetCore.Relay
{
    public class ObjectResolversContianer
    {
        ConcurrentDictionary<string, Type> _resolvers;

        public ObjectResolversContianer(IAssemblyResolver assemblyResolver)
        {
            var resolvers = assemblyResolver
               .GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x => typeof(INodeResolver).IsAssignableFrom(x));

            _resolvers = new ConcurrentDictionary<string, Type>();

            foreach (var resolver in resolvers)
            {
                var genericParameter = resolver.GetGenericArguments()[0];
                var nameAttr = genericParameter.GetAttribute<NameAttribute>();
                var name = nameAttr?.Name ?? genericParameter.Name;
                _resolvers.TryAdd(name, resolver);
            }
        }

        public Type GetResolver(string typeName)
        {
            return _resolvers[typeName];
        }

    }
}
