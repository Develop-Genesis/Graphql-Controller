using GraphqlController.AspNetCore.PersistedQuery;
using GraphqlController.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPersistedQuery(this IServiceCollection services)
        {            
            services.AddScoped<IPersistedQueryService, PersistedQueryService>();
            services.AddScoped<ExecutionPersistedQueryMiddleware>();

            return services;
        }

        public static IServiceCollection AddPersistedGraphqlQuerySingleton<T>(this IServiceCollection services) where T : class, IPersistedQueryCache
           => services.AddSingleton<IPersistedQueryCache, T>();

        public static IServiceCollection AddPersistedGraphqlQueryScoped<T>(this IServiceCollection services) where T : class, IPersistedQueryCache
           => services.AddScoped<IPersistedQueryCache, T>();

        public static IServiceCollection NotUsePersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQuerySingleton<NotSupportedPersistedQuery>();

        /// <summary>
        /// Recomended only for testing
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryPersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQueryScoped<InMemoryPersistedQueryCache>();

        public static IServiceCollection AddDistributedPersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQueryScoped<DistributedPersistedQueryCache>();

    }
}
