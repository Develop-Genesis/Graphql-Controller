using System;
using System.Collections.Generic;
using System.Text;
using GraphqlController.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace GraphqlController.AspNetCore
{
    public static class EndpointBuilderExtensions
    {
        public static void MapGraphQLEnpoint<T>(this IEndpointRouteBuilder endpoints, string path)
            => endpoints.MapGraphQLEnpoint(path, typeof(T));

        public static void MapGraphQLEnpoint(this IEndpointRouteBuilder endpoints, string path, Type rootType)
        {
            var schemaRoutes = (ISchemaRouteService)endpoints.ServiceProvider.GetService(typeof(ISchemaRouteService));

            schemaRoutes.AddRoute(path, rootType);

            endpoints.MapControllerRoute(
                name: path,
                pattern: path,
                defaults: new { controller = "GraphQL", action = "ExecuteQuery" });
        }
    }
}
