using GraphQL.Resolvers;
using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Helpers;
using LoxSmoke.DocXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.GraphQl
{
    public class DynamicGraphType : ObjectGraphType
    {
        public DynamicGraphType(IGraphQlTypePool graphTypePool, Type type, bool isRoot = false)
        {
            if (!type.IsClass || type.IsInterface)
            {
                throw new ArgumentException("Invalid object type");
            }

            // get the name
            var nameAttr = type.GetAttribute<NameAttribute>();
            var descAttr = type.GetAttribute<DescriptionAttribute>();

            // set type name and description
            Name = nameAttr?.Name ?? type.Name;
            Description = descAttr?.Description ?? DocXmlHelper.DocReader.GetTypeComments(type).Summary;

            // set the type metadata
            Metadata["type"] = type;

            {
                // sets the custom metadatas
                var metadatas = Attribute.GetCustomAttributes(type, typeof(MetadataAttribute));
                foreach (MetadataAttribute metadata in metadatas)
                {
                    Metadata[metadata.Key] = metadata.Value;
                }
            }

            // Check for interfaces
            var interfaces = type.GetNotDerivedInterfaces();

            // Add all the interface that this implement
            foreach (var intrfce in interfaces)
            {
                AddResolvedInterface(graphTypePool.GetGraphType(intrfce) as IInterfaceGraphType);
            }

            // Implementing isTypeOf in the case this type implement an interface
            IsTypeOf = obj => obj.GetType() == type;

            // Generate fields -----------------------------------------------
            // start with the properties
            var properties = type
                // Get all properties with getters
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
                // ignore the ones that have the ignore attribute
                .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

            foreach (var property in properties)
            {
                Type graphTypeAsType = null;
                IGraphType graphType = null;
                var customTypeAttr = property.GetAttribute<CustomGraphTypeAttribute>();
                if(customTypeAttr != null)
                {
                    if(customTypeAttr.TypeName != null)
                    {
                        graphType = graphTypePool.GetCustomType(customTypeAttr.TypeName);
                    }
                    else
                    {
                        graphTypeAsType = customTypeAttr.Type;
                    }
                }
                else
                {
                    graphType = graphTypePool.GetGraphType(property.PropertyType);
                }
                
                var descriptionAttr = property.GetAttribute<DescriptionAttribute>();
                var fieldNameAttr = property.GetAttribute<NameAttribute>();
                var isNonNull = property.GetAttribute<NonNullAttribute>() != null;

                // create field
                var field = new FieldType()
                {
                    Name = fieldNameAttr == null ? property.Name : fieldNameAttr.Name,
                    Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(property).Summary,
                    Type = graphTypeAsType,
                    ResolvedType = graphType == null ? null : (isNonNull ? new NonNullGraphType(graphType) : graphType),                    
                    Resolver = new FuncFieldResolver<object>(c => GraphQlHelpers.GetFinalValue(property.GetValue(GraphQlHelpers.GetSourceInstance(c, type, isRoot)))),                    
                };

                // add the .net type of this field in the metadata
                field.Metadata["type"] = property;

                var metadatas = Attribute.GetCustomAttributes(type, typeof(MetadataAttribute));
                foreach (MetadataAttribute metadata in metadatas)
                {
                    Metadata[metadata.Key] = metadata.Value;
                }

                AddField(field);
            }

            // work with the methods
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                              .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

            // for each method public
            foreach (var method in methods)
            {
                if (method.Name == nameof(GraphNodeType<object>.OnCreateAsync) ||
                   method.IsSpecialName)
                {
                    continue;
                }

                var descriptionAttr = method.GetAttribute<DescriptionAttribute>();
                var fieldNameAttr = method.GetAttribute<NameAttribute>();

                Type graphTypeAsType = null;
                IGraphType graphType = null;
                IFieldResolver resolver;
                bool graphTypeResolved = false;

                var customTypeAttr = method.GetAttribute<CustomGraphTypeAttribute>();
                if (customTypeAttr != null)
                {
                    if (customTypeAttr.TypeName != null)
                    {
                        graphType = graphTypePool.GetCustomType(customTypeAttr.TypeName);
                    }
                    else
                    {
                        graphTypeAsType = customTypeAttr.Type;
                    }
                    graphTypeResolved = true;
                }

                if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    if(!graphTypeResolved)
                    {
                        var awaitableReturnType = method.ReturnType.GetGenericArguments()[0];
                        graphType = graphTypePool.GetGraphType(awaitableReturnType);
                    }

                    resolver = new AsyncFieldResolver<object>(async c =>
                    {
                        var task = GraphQlHelpers.ExecuteResolverFunction(method, c, type, isRoot);
                        await ((Task)task);

                        var resultProp = task.GetType().GetProperty(nameof(Task<object>.Result));
                        var result = resultProp.GetValue(task);

                        return result;
                    });
                }
                else
                {
                    if (!graphTypeResolved)
                    {
                        graphType = graphTypePool.GetGraphType(method.ReturnType);
                    }
                    
                    resolver = new FuncFieldResolver<object>(c => GraphQlHelpers.ExecuteResolverFunction(method, c, type, isRoot));
                }

                var isNonNull = method.GetAttribute<NonNullAttribute>() != null;

                // create field
                var field = new FieldType()
                {
                    Arguments = GraphQlHelpers.GetArguments(graphTypePool, method),
                    Name = fieldNameAttr == null ? method.Name : fieldNameAttr.Name,
                    Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(method).Summary,
                    Type = graphTypeAsType,
                    ResolvedType = graphType == null ? null : (isNonNull ? new NonNullGraphType(graphType) : graphType),
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
