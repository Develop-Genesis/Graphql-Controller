using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGraphqlCache(this IServiceCollection services, CacheConfiguration cacheConfig = null)
        {
            if (cacheConfig == null)
            {
                cacheConfig = new CacheConfiguration();
            }
            else
            {
                if (cacheConfig.DefaultMaxAge < 0)
                {
                    throw new InvalidOperationException("Default Max Age cannot be less than 0");
                }
            }

            services.AddSingleton(cacheConfig);

            services.AddScoped<IGraphqlInAppCacheService, GraphqlInAppCacheService>();
            services.AddScoped<ICachePolicy, DefaultCachePolicy>();
            services.AddScoped<GraphQlExecutionMiddleware>();

            return services;
        }
    }
}
