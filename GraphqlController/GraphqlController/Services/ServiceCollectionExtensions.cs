using GraphQL.Types;
using GraphqlController.GraphQl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GraphqlController.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQlController(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetCallingAssembly() };

            // add the service provider
            services.AddScoped<IScopedServiceProviderResolver>(c => new ScopedServiceProvider(c));
            // add the graphql node types creator
            services.AddScoped<IGraphqlCreator, GraphqlCreator>();
            // add the graphql types pools
            services.AddSingleton<IGraphQlTypePool, GraphQlTypePool>();
            // add the assembly resolver
            services.AddSingleton<IAssemblyResolver>(new AssemblyResolver(assemblies));
            // Register the types
            services.RegisterAllTypes(assemblies);

            // register the schema
            services.AddSingleton<ISchema, DynamicSchema>();
            
        }

        /// <summary>
        /// Register all types
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        private static void RegisterAllTypes(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Add all type nodes to the container as transient
            foreach (var assembly in assemblies)
            {
                var nodeTypes = assembly.GetTypes().Where(x => typeof(IGraphNodeType).IsAssignableFrom(x));
                foreach (var type in nodeTypes)
                {
                    services.AddTransient(type);
                }
            }
        }        

    }
}
