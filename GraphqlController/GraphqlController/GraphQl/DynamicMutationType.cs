using GraphQL.Resolvers;
using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.GraphQl
{
    public class DynamicMutationType : ObjectGraphType
    {
        public DynamicMutationType(IGraphQlTypePool graphTypePool, IEnumerable<Type> types)
        {
            Name = "Mutations";
            Description = "Contains the mutation of the graphql api";

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsInterface)
                {
                    throw new ArgumentException("Invalid subscription type");
                }

                // work with the methods
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                  .Where(x => !(x.DeclaringType.IsGenericType && x.DeclaringType.GetGenericTypeDefinition() == typeof(GraphNodeType<>)))
                                  .Where(x => x.DeclaringType != typeof(GraphNodeType))
                                  .Where(x => x.DeclaringType != typeof(object))
                                  .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

                foreach (var method in methods)
                {
                    if (method.Name == nameof(GraphNodeType<object>.OnCreateAsync) ||
                   method.IsSpecialName)
                    {
                        continue;
                    }

                    var descriptionAttr = method.GetAttribute<DescriptionAttribute>();
                    var fieldNameAttr = method.GetAttribute<NameAttribute>();

                    IGraphType graphType;
                    IFieldResolver resolver;
                    if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var awaitableReturnType = method.ReturnType.GetGenericArguments()[0];
                        graphType = graphTypePool.GetGraphType(awaitableReturnType);
                        resolver = new AsyncFieldResolver<object>(async c =>
                        {
                            var task = GraphQlHelpers.ExecuteResolverFunction(method, c, type, true);
                            await ((Task)task);

                            var resultProp = task.GetType().GetProperty(nameof(Task<object>.Result));
                            var result = resultProp.GetValue(task);

                            return result;
                        });
                    }
                    else
                    {
                        graphType = graphTypePool.GetGraphType(method.ReturnType);
                        resolver = new FuncFieldResolver<object>(c => GraphQlHelpers.ExecuteResolverFunction(method, c, type, true));
                    }

                    var isNonNull = method.GetAttribute<NonNullAttribute>() != null;

                    // create field
                    var field = new FieldType()
                    {
                        Arguments = GraphQlHelpers.GetArguments(graphTypePool, method),
                        Name = fieldNameAttr == null ? method.Name : fieldNameAttr.Name,
                        Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(method).Summary,
                        ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType,
                        Resolver = resolver
                    };

                    // add the .net type of this field in the metadata
                    field.Metadata["type"] = method;

                    var metadatas = Attribute.GetCustomAttributes(type, typeof(MetadataAttribute));
                    foreach (MetadataAttribute metadata in metadatas)
                    {
                        Metadata[metadata.Key] = metadata.Value;
                    }

                    AddField(field);
                }
            }
        }
    }
}
