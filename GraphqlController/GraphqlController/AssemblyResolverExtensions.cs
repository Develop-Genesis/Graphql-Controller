using GraphqlController.Attributes;
using GraphqlController.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController
{
    public static class AssemblyResolverExtensions
    {
        public static IEnumerable<Type> GetRootTypes(this IAssemblyResolver assemblyResolver)
          => assemblyResolver.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IGraphNodeType).IsAssignableFrom(x))
                .Where(x => Attribute.GetCustomAttribute(x, typeof(RootTypeAttribute)) != null);

    }
}
