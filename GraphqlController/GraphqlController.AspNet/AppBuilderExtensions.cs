using GraphQL.Types;
using GraphqlController.AspNetCore.Authorization;
using GraphqlController.AspNetCore.Services;
using GraphqlController.Execution;
using GraphqlController.Services;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public static class AppBuilderExtensions
    {

        public static IApplicationBuilder AddSchemaInitializer(this IApplicationBuilder app, Action<Schema> initializer)
        {
            var schemaResolver = (ISchemaResolver)app.ApplicationServices.GetService(typeof(ISchemaResolver));
            schemaResolver.AddIntializer(initializer);
            return app;
        }

        public static IApplicationBuilder UseGraphQLController(this IApplicationBuilder app)
        {
            var schemaResolver = (ISchemaResolver)app.ApplicationServices.GetService(typeof(ISchemaResolver));
            schemaResolver.BuildSchemas();

            return app;
        }

        public static IGraphQLExecutionBuilder UseGraphQlExecutionFor(this IApplicationBuilder app, Type root)
        {
            var executionBuilderResolver = (IExecutionBuilderResolver)app.ApplicationServices.GetService(typeof(IExecutionBuilderResolver));
            var builder = executionBuilderResolver.GetGraphqlExecutionBuilder(root);
            
            // add authorization as a built in feature
            builder.Use<AuthorizationExecutionMiddleware>();

            return builder;
        }

        public static IGraphQLExecutionBuilder UseGraphQlExecutionFor<T>(this IApplicationBuilder app)
            => app.UseGraphQlExecutionFor(typeof(T));

    }
}
