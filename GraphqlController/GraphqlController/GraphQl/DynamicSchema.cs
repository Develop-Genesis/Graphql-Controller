using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicSchema : Schema
    {
        public DynamicSchema(IGraphQlTypePool pool, IAssemblyResolver assemblyResolver)
        {
            var rootType = assemblyResolver.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IGraphNodeType).IsAssignableFrom(x))
                .FirstOrDefault(x => Attribute.GetCustomAttribute(x, typeof(RootTypeAttribute)) != null);

            if(rootType == null)
            {
                throw new InvalidOperationException("Cannot find Root type, make sure that it is a valid GraphNodeType and has RootType attriobute");
            }

            var graphqlRootType = pool.GetRootGraphType(rootType);

            Query = graphqlRootType as IObjectGraphType;
        }
    }
}
