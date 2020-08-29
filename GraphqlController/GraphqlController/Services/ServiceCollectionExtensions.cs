using GraphQL.Types;
using GraphqlController.Execution;
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
        public static GraphqlControllerServiceBuilder AddGraphQlController(this IServiceCollection services)
        {            
            // add the service provider
            services.AddScoped<IScopedServiceProviderResolver>(c => new ScopedServiceProvider(c));
            // add the graphql node types creator
            services.AddScoped<IGraphqlResolver, GraphqlResolver>();
            // add the graphql types pools
            services.AddSingleton<IGraphQlTypePool, GraphQlTypePool>();            
                        
            // add the schema resolver service
            services.AddSingleton<ISchemaResolver, SchemaResolver>();

            // add the executer
            services.AddScoped<IGraphQLExecutor, GraphQlExecutor>();

            // create the builder
            var builder = new GraphqlControllerServiceBuilder(services);

            return builder;
        }

        

    }
}
