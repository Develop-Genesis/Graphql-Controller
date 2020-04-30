using GraphQL.Instrumentation;
using GraphqlController.AspNetCore.Cache;
using GraphqlController.AspNetCore.PersistedQuery;
using GraphqlController.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GraphqlController.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGraphQlEndpoint(this IServiceCollection services, CacheConfiguration cacheConfig = null)
        {
            if (cacheConfig == null)
            {
                cacheConfig = new CacheConfiguration();
            }
            else
            {
                if(cacheConfig.DefaultMaxAge < 0)
                {
                    throw new InvalidOperationException("Default Max Age cannot be less than 0");
                }
            }

            services.AddSingleton<ISchemaRouteService, SchemaRouteService>();
            services.AddScoped<IPersistedQueryService, PersistedQueryService>();

            services.AddSingleton<IFieldMiddlewareBuilder>(new FieldMiddlewareBuilder());

            services.AddSingleton(cacheConfig);

            services.AddScoped<IGraphqlInAppCacheService, GraphqlInAppCacheService>();
            services.AddScoped<ICachePolicy, DefaultCachePolicy>();

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
            => services.AddPersistedGraphqlQueryScoped<InMemoryPersistedQueryCache>();

        public static IServiceCollection AddDistributedPersistedQuery(this IServiceCollection services)
            => services.AddPersistedGraphqlQueryScoped<DistributedPersistedQueryCache>();



    }
}
