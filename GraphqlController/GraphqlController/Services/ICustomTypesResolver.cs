using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.Services
{
    public interface ICustomTypesResolver
    {
        IEnumerable<IGraphType> GetCustomTypes();
    }

    public static class CustomTypesResolverExtensions
    {
        public static IGraphType GetCustomType(this ICustomTypesResolver resolver, string name)
            => resolver.GetCustomTypes().FirstOrDefault(x => x.Name == name);
    }

}
