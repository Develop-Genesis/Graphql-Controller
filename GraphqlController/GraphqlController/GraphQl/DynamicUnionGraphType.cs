using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicUnionGraphType : UnionGraphType
    {

        private static HashSet<Type> UnionTypes = new HashSet<Type>()
        {
            typeof(Union<,>),
            typeof(Union<,,>),
            typeof(Union<,,>),
            typeof(Union<,,,>),
            typeof(Union<,,,,>),
            typeof(Union<,,,,,>),
            typeof(Union<,,,,,,>),
            typeof(Union<,,,,,,,>),
            typeof(Union<,,,,,,,,>),
            typeof(Union<,,,,,,,,,>)
        };

        public DynamicUnionGraphType(IGraphQlTypePool pool, Type type)
        {
            if (!typeof(IUnionGraphType).IsAssignableFrom(type))
                throw new InvalidOperationException("Invalid Union type");

            if( IsUnionType(type) )
            {
                // Generate name
                var genericArguments = type.GetGenericArguments();
                Name = string.Join('_', genericArguments.Select(x => x.Name)); 
            } 
            else
            {
                if( !IsUnionType(type.BaseType) )
                    throw new InvalidOperationException("Invalid Union type");

                var nameAttr = type.GetAttribute<NameAttribute>();
                var descAttr = type.GetAttribute<DescriptionAttribute>();

                Name = nameAttr?.Name ?? type.Name;
                Description = descAttr?.Description ?? DocXmlHelper.DocReader.GetTypeComments(type).Summary;
                
                type = type.BaseType;
            }

            {
                var genericArguments = type.GetGenericArguments();

                foreach (var genericArgument in genericArguments)
                {
                    AddPossibleType(pool.GetGraphType(genericArgument) as IObjectGraphType);
                }
            }
        }

        private static bool IsUnionType(Type type)
        {
            if (!type.IsGenericType)
            {
                return false;
            }

            var genericDefinition = type.GetGenericTypeDefinition();

            return UnionTypes.Contains(genericDefinition);
        }

    }
}
  