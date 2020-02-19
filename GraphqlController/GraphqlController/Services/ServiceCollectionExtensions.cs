using GraphqlController.GraphQl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQlController(this IServiceCollection services)
        {
            services.AddScoped<IScopedServiceProviderResolver>(c => new ScopedServiceProvider(c));
            services.AddScoped<IGraphqlCreator, GraphqlCreator>();

            services.AddSingleton<IGraphQlTypePool, GraphQlTypePool>();
        }
    }
}
