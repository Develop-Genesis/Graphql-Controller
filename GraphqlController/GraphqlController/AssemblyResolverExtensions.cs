using GraphqlController.Attributes;
using GraphqlController.Helpers;
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
                .Where(x => x.GetAttribute<RootTypeAttribute>() != null);

        public static IEnumerable<Type> GetSubscriptionTypes(this IAssemblyResolver assemblyResolver, Type rootType)
          => assemblyResolver.GetAssemblies()
                             .SelectMany(x => x.GetTypes())
                             .Where(x => typeof(IGraphNodeType).IsAssignableFrom(x))
                             .Where(x => x.GetAttribute<SubscriptionAttribute>() != null)
                             .Where(x => x.GetAttribute<SubscriptionAttribute>().Root == rootType);

        public static IEnumerable<Type> GetMutationTypes(this IAssemblyResolver assemblyResolver, Type rootType)
          => assemblyResolver.GetAssemblies()
                             .SelectMany(x => x.GetTypes())
                             .Where(x => typeof(IGraphNodeType).IsAssignableFrom(x))
                             .Where(x => x.GetAttribute<MutationAttribute>() != null)
                             .Where(x => x.GetAttribute<MutationAttribute>().Root == rootType);
    }
}
