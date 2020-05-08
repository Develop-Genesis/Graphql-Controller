using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public static class GraphqlExecutionBuilderExtensions
    {
        public static IGraphQLExecutionBuilder UseCache(this IGraphQLExecutionBuilder builder)
        {
            builder.Use<GraphQlExecutionMiddleware>();
            return builder;
        }
    }
}
