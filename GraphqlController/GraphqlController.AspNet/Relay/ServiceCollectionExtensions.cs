using GraphQL;
using GraphqlController.Services;
using GraphqlController.AspNetCore.Relay.Types;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.AspNetCore.Relay
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRelay(this GraphqlControllerServiceBuilder builder)
        {
            builder.Services.AddSingleton<GlobalIdType>();
            builder.AddCustomScalarType(typeof(GlobalId), new GlobalIdType());
            ValueConverter.Register(typeof(string), typeof(GlobalId), ConvertStringToGlobalId);

            var resolvers = builder.AssemblyResolver
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(INodeResolver).IsAssignableFrom(x));

            foreach(var resolver in resolvers)
            {
                builder.Services.AddTransient(resolver);
            }

            builder.AddAssembly(typeof(INode).Assembly);

            builder.Services.AddSingleton<ObjectResolversContianer>();
            builder.Services.AddTransient<IObjectResolverService, ObjectResolverService>();
        }        

        private static object ConvertStringToGlobalId(object arg)
        {
            var str = arg as string;
            return new GlobalId(str);
        }
    }
}
