using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.GraphQl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.Helpers
{
    internal static class GraphQlHelpers
    {

        internal static object GetSourceInstance(ResolveFieldContext<object> c, Type type, bool isRoot) =>
            isRoot
            ? (c.UserContext["serviceProvider"] as IServiceProvider).GetService(type)
            : c.Source;

        internal static QueryArguments GetArguments(IGraphQlTypePool graphTypePool, MethodInfo methodInfo)
        {
            var result = new List<QueryArgument>();
            var parameters = methodInfo.GetParameters();
            foreach (var param in parameters)
            {
                if (param.ParameterType == typeof(CancellationToken))
                {
                    continue;
                }

                var argumentNameAttr = param.GetAttribute<NameAttribute>();
                var argumentDescriptionAttr = param.GetAttribute<DescriptionAttribute>();
                var isNonNullType = param.GetAttribute<NonNullAttribute>() != null;

                var name = argumentNameAttr == null ? param.Name : argumentNameAttr.Name;
                var description = argumentDescriptionAttr?.Description ??
                    DocXmlHelper.DocReader.GetMethodComments(methodInfo)?.Parameters.FirstOrDefault(x => x.Name == param.Name).Text;

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

        internal static object ExecuteResolverFunction(MethodInfo method, ResolveFieldContext<object> c, Type type, bool isRoot)
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
        internal static object GetFinalValue(object result)
            => result == null ? null :
               typeof(IUnionGraphType).IsAssignableFrom(result.GetType())
               ? (result as IUnionGraphType).Value
               : result;

        // Get deafult value of type
        internal static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
