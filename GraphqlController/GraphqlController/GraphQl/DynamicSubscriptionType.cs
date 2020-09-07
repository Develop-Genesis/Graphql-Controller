using GraphQL.Resolvers;
using GraphQL.Subscription;
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
    public class DynamicSubscriptionType : ObjectGraphType
    {
        public DynamicSubscriptionType(IGraphQlTypePool graphTypePool, IEnumerable<Type> types)
        {
            Name = "Subscriptions";
            Description = "Contains the subscriptions of the graphql api";

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsInterface)
                {
                    throw new ArgumentException("Invalid subscription type");
                }

                // Generate fields -----------------------------------------------
                // start with the properties
                var properties = type
                    // Get all properties with getters
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty)
                    // ignore the ones that have the ignore attribute
                    .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

                foreach (var property in properties)
                {
                    var observableType = property.PropertyType.GetInterfacesIncludingType()
                                                              .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition().Equals(typeof(IObservable<>)));

                    if (observableType == null)
                    {
                        throw new InvalidOperationException("Subscription property can only return or have type IObservable<>, derive from it or have the IgnoreAttribute");
                    }

                    var graphType = graphTypePool.GetGraphType(observableType.GetGenericArguments()[0]);
                    var descriptionAttr = property.GetAttribute<DescriptionAttribute>();
                    var fieldNameAttr = property.GetAttribute<NameAttribute>();
                    var isNonNull = property.GetAttribute<NonNullAttribute>() != null;

                    var field = new EventStreamFieldType()
                    {
                        Name = fieldNameAttr == null ? property.Name : fieldNameAttr.Name,
                        Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(property).Summary,
                        ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType,
                        Resolver = new FuncFieldResolver<object>(ResolveObject),
                        Subscriber = new EventStreamResolver<object>(c => Subscribe(c, property, type))
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
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                  .Where(x => x.DeclaringType != typeof(object))
                                  .Where(x => x.GetAttribute<IgnoreAttribute>() == null);

                foreach (var method in methods)
                {
                    if (method.IsSpecialName)
                    {
                        continue;
                    }

                    var descriptionAttr = method.GetAttribute<DescriptionAttribute>();
                    var fieldNameAttr = method.GetAttribute<NameAttribute>();

                    IGraphType graphType;
                    IEventStreamResolver subscriber = null;
                    IAsyncEventStreamResolver subscriberAsync = null;
                    if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var observable = method.ReturnType.GetGenericArguments()[0];

                        var observableInterface = observable.GetInterfacesIncludingType()
                                                        .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition().Equals(typeof(IObservable<>)));

                        if (observableInterface == null)
                        {
                            throw new InvalidOperationException("Subscription property can only return or have type IObservable<>, derive from it or have the IgnoreAttribute");
                        }

                        var observableType = observableInterface.GetGenericArguments()[0];

                        graphType = graphTypePool.GetGraphType(observableType);
                        subscriberAsync = new AsyncEventStreamResolver<object>(async c =>
                        {
                            var task = GraphQlHelpers.ExecuteResolverFunction(method, c, type, true);
                            await ((Task)task);

                            var resultProp = task.GetType().GetProperty(nameof(Task<object>.Result));
                            var result = resultProp.GetValue(task);

                            return result as IObservable<object>;
                        });
                    }
                    else
                    {
                        var observable = method.ReturnType;

                        var observableInterface = observable.GetInterfacesIncludingType()
                                                       .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition().Equals(typeof(IObservable<>)));

                        var observableType = observableInterface.GetGenericArguments()[0];

                        graphType = graphTypePool.GetGraphType(observableType);
                        subscriber = new EventStreamResolver<object>(c => GraphQlHelpers.ExecuteResolverFunction(method, c, type, true) as IObservable<object>);
                    }

                    var isNonNull = method.GetAttribute<NonNullAttribute>() != null;

                    // create field
                    var field = new EventStreamFieldType()
                    {
                        Arguments = GraphQlHelpers.GetArguments(graphTypePool, method),
                        Name = fieldNameAttr == null ? method.Name : fieldNameAttr.Name,
                        Description = descriptionAttr?.Description ?? DocXmlHelper.DocReader.GetMemberComments(method).Summary,
                        ResolvedType = isNonNull ? new NonNullGraphType(graphType) : graphType,
                        Resolver = new FuncFieldResolver<object>(ResolveObject),
                        Subscriber = subscriber,
                        AsyncSubscriber = subscriberAsync
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

        private static object ResolveObject(ResolveFieldContext c)
             => c.Source;
        // new FuncFieldResolver<object>(c => GraphQlHelpers.GetFinalValue(property.GetValue(GraphQlHelpers.GetSourceInstance(c, type, false))))          

        private static IObservable<object> Subscribe(ResolveEventStreamContext c, PropertyInfo property, Type type)
        {
            var observable = property.GetValue(GraphQlHelpers.GetSourceInstance(c, type, true));
            return observable as IObservable<object>;
        }

    }
}
