using GraphqlController.AspNetCore.PersistedQuery;
using GraphqlController.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GraphqlController.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGraphQlEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<ISchemaRouteService, SchemaRouteService>();
            services.AddScoped<IPersistedQueryService, PersistedQueryService>();

            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly())
                                     .AddControllersAsServices();

            return services;
        }

        public static IServiceCollection AddPersistedGraphqlQuerySingleton<T>(this IServiceCollection services) where T : class, IPersistedQueryCahce
           => services.AddSingleton<IPersistedQueryCahce, T>();

        public static IServiceCollection AddPersistedGraphqlQueryScoped<T>(this IServiceCollection services) where T : class, IPersistedQueryCahce
           => services.AddScoped<IPersistedQueryCahce, T>();

        public static IServiceCollection NotUsePersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQuerySingleton<NotSupportedPersistedQuery>();

        /// <summary>
        /// Recomended only for testing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryPersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQuerySingleton<InMemoryPersistedQueryCache>();

    }
}
