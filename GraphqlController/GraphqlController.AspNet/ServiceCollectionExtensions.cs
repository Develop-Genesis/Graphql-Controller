using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace GraphqlController.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGraphQlEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<ISchemaRouteService, SchemaRouteService>();

            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly())
                                     .AddControllersAsServices();                                    
        }
    }
}
