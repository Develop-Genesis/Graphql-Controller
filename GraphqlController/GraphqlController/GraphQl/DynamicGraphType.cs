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
                var graphType = graphTypePool.GetGraphType(property.PropertyType);
                var descriptionAttr = property.GetAttribute<DescriptionAttribute>();
                var fieldNameAttr = property.GetAttribute<NameAttribute>();
                var isNonNull = property.GetAttribute<NonNullAttribute>() != null;

                // create field
                var field = new FieldType()
                {
                    Name = fieldNameAttr == null ? property.Name : fieldNameAttr.Name,
                    Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(property).Summary,
                    ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType,
                    Resolver = new FuncFieldResolver<object>(c => GetFinalValue(property.GetValue(GetSourceInstance(c, type, isRoot)))),                    
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

                IGraphType graphType;
                IFieldResolver resolver;
                if (method.IsGenericMethod && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var awaitableReturnType = method.ReturnType.GetGenericArguments()[0];
                    graphType = graphTypePool.GetGraphType(awaitableReturnType);
                    resolver = new AsyncFieldResolver<object>(c =>
                    {
                        var task = ExecuteResolverFunction(method, c, type, isRoot);
                        return task as Task<object>;
                    });
                }
                else
                {
                    graphType = graphTypePool.GetGraphType(method.ReturnType);
                    resolver = new FuncFieldResolver<object>(c => ExecuteResolverFunction(method, c, type, isRoot));
                }

                var isNonNull = method.GetAttribute<NonNullAttribute>() != null;

                // create field
                var field = new FieldType()
                {
                    Arguments = GetArguments(graphTypePool, method),
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

        private static object GetSourceInstance(ResolveFieldContext c, Type type, bool isRoot) =>
            isRoot
            ? (c.UserContext["serviceProvider"] as IServiceProvider).GetService(type)
            : c.Source;

        private static QueryArguments GetArguments(IGraphQlTypePool graphTypePool, MethodInfo methodInfo)
        {
            var result = new List<QueryArgument>();
            var parameters = methodInfo.GetParameters();
            foreach(var param in parameters)
            {
                if(param.ParameterType == typeof(CancellationToken))
                {
                    continue;
                }

                var argumentNameAttr = param.GetAttribute<NameAttribute>();
                var argumentDescriptionAttr = param.GetAttribute<DescriptionAttribute>();
                var isNonNullType = param.GetAttribute<NonNullAttribute>() != null;

                var name = argumentNameAttr == null ? param.Name : argumentNameAttr.Name;
                var description = argumentDescriptionAttr?.Description ?? 
                    DocXmlHelper.DocReader.GetMethodComments(methodInfo)?.Parameters.First(x => x.Name == param.Name).Text;

                var graphInputType = isNonNullType
                                            ? new NonNullGraphType(graphTypePool.GetInputType(param.ParameterType))
                                            : graphTypePool.GetInputType(param.ParameterType);

                var argument = new QueryArgument(graphInputType) 
                { 
                    Name = name, 
                    Description = description,                    
                    DefaultValue = param.HasDefaultValue ? param.DefaultValue : null
                };

                result.Add(argument);
            }

            return new QueryArguments(result);
        }

        private static object ExecuteResolverFunction(MethodInfo method, ResolveFieldContext c, Type type, bool isRoot)
        {
            var parameters = method.GetParameters();
            var parameterValues = new List<object>();

            foreach (var param in parameters)
            {
                if (param.ParameterType == typeof(CancellationToken))
                {
                    parameterValues.Add(c.CancellationToken);
                    continue;
                }

                var nameAttr = param.GetAttribute<NameAttribute>();

                object defaultValue;
                if (param.HasDefaultValue)
                {
                    defaultValue = param.DefaultValue;
                }
                else
                {
                    defaultValue = GetDefault(param.ParameterType);
                }

                var value = c.GetArgument(param.ParameterType, nameAttr == null ? param.Name : nameAttr.Name, defaultValue);
                parameterValues.Add(value);
            }

            var resultValue = method.Invoke(GetSourceInstance(c, type, isRoot), parameterValues.ToArray());

            return GetFinalValue(resultValue);
        }

        // check final result
        private static object GetFinalValue(object result)
            => typeof(IUnionGraphType).IsAssignableFrom(result.GetType())
               ? (result as IUnionGraphType).Value
               : result;

        // Get deafult value of type
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
