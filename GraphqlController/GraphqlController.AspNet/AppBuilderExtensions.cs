using GraphqlController.Services;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public static class AppBuilderExtensions
    {
        public static void UseGraphQLController(this IApplicationBuilder app)
        {
            var schemaResolver = (ISchemaResolver)app.ApplicationServices.GetService(typeof(ISchemaResolver));
            schemaResolver.BuildSchemas();
        }
    }
}
