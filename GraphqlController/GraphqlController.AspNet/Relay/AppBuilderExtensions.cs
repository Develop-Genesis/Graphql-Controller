using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using GraphqlController.AspNetCore;

namespace GraphqlController.AspNetCore.Relay
{
    public static class AppBuilderExtensions
    {
        public static void AddGraphqlRelay(this IApplicationBuilder app)
        {
            app.AddSchemaInitializer(schema =>
            {
                schema.RegisterValueConverter(new GlobalIdAstValueConverter());
            });
        }
    }
}
