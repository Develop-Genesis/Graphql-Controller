using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public static class GraphqlExecutionBuilderExtensions
    {
        public static IGraphQLExecutionBuilder UsePersistedQuery(this IGraphQLExecutionBuilder builder)
        {
            builder.Use<ExecutionPersistedQueryMiddleware>();
            return builder;
        }
    }
}
