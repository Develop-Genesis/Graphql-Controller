﻿using GraphQL.Instrumentation;
using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
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
            

            services.AddSingleton<IFieldMiddlewareBuilder>(new FieldMiddlewareBuilder());

            services.AddSingleton<IExecutionBuilderResolver, ExecutionBuilderResolver>();

            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly())
                                     .AddControllersAsServices();

            return services;
        }




    }
}
