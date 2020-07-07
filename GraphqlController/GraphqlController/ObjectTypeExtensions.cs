using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController
{
    public static class ObjectTypeExtensions
    {
        /// <summary>
        /// iterates in the tree to list all the interfaces in the object
        /// </summary>
        public static IEnumerable<InterfaceGraphType> GetAllInterfaces(this IObjectGraphType objectGraphType)
          => objectGraphType.GetAllTypes().Where(x => x is InterfaceGraphType).Select(x => x as InterfaceGraphType);

        /// <summary>
        /// iterates in the tree to list all the types in the object
        /// </summary>
        public static IEnumerable<IGraphType> GetAllTypes(this IObjectGraphType objectGraphType)
        {
            Queue<IGraphType> typeQueue = new Queue<IGraphType>();

            HashSet<string> typeMark = new HashSet<string>();

            typeMark.Add(objectGraphType.Name);
            yield return objectGraphType;

            foreach(var field in objectGraphType.Fields)
            {
                var resolvedType = GetNonFeatureGraphType(field.ResolvedType);

                if(resolvedType is IObjectGraphType || resolvedType is InterfaceGraphType)
                {
                    if(!typeMark.Contains(resolvedType.Name))
                    {
                        typeQueue.Enqueue(resolvedType);

                        typeMark.Add(resolvedType.Name);
                        yield return resolvedType;
                    }                      
                } else
                {
                    if (!typeMark.Contains(resolvedType.Name))
                    {
                        typeMark.Add(resolvedType.Name);
                        yield return resolvedType;
                    }
                }
            }

            while(typeQueue.Count > 0)
            {
                var type = typeQueue.Dequeue();

                IEnumerable<FieldType> fields;
                switch(type)
                {
                    case InterfaceGraphType list:
                        fields = list.Fields;
                        break;

                    case IObjectGraphType objType:
                        fields = objType.Fields;
                        break;

                    default:
                        fields = Array.Empty<FieldType>();
                        break;
                }

                foreach (var field in fields)
                {
                    var resolvedType = GetNonFeatureGraphType(field.ResolvedType);

                    if (resolvedType is IObjectGraphType || resolvedType is InterfaceGraphType)
                    {
                        if (!typeMark.Contains(resolvedType.Name))
                        {
                            typeQueue.Enqueue(resolvedType);

                            typeMark.Add(resolvedType.Name);
                            yield return resolvedType;
                        }
                    }
                    else
                    {
                        if (resolvedType != null && !typeMark.Contains(resolvedType.Name))
                        {
                            typeMark.Add(resolvedType.Name);
                            yield return resolvedType;
                        }
                    }
                }

            }

        }

        static IGraphType GetNonFeatureGraphType(IGraphType graphType)
        {
            switch (graphType)
            {
                case ListGraphType list:
                    return GetNonFeatureGraphType(list.ResolvedType);

                case NonNullGraphType nonNull:
                    return GetNonFeatureGraphType(nonNull.ResolvedType);
            }

            return graphType;
        }

    }
}
