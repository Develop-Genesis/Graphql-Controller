using GraphQL.Instrumentation;
using GraphqlController.AspNetCore.Cache;
using GraphqlController.Services;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseGraphQLController(this IApplicationBuilder app)
        {
            var schemaResolver = (ISchemaResolver)app.ApplicationServices.GetService(typeof(ISchemaResolver));
            schemaResolver.BuildSchemas();

            // add cache field middleware
            app.WithGraphqlFieldMiddleware()
                .Use<CacheFieldMiddleware>();

            return app;
        }

        public static IFieldMiddlewareBuilder WithGraphqlFieldMiddleware(this IApplicationBuilder app)
        {
            var builder = (IFieldMiddlewareBuilder)app.ApplicationServices.GetService(typeof(IFieldMiddlewareBuilder));
            return builder;
        }

    }
}
