using GraphqlController.Execution;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Services
{
    public class ExecutionBuilderResolver : IExecutionBuilderResolver
    {
        Dictionary<Type, IGraphQLExecutionBuilder> _executionBuilders = new Dictionary<Type, IGraphQLExecutionBuilder>();

        public IGraphQLExecutionBuilder GetGraphqlExecutionBuilder(Type root)
        {
            IGraphQLExecutionBuilder result;
            if (!_executionBuilders.TryGetValue(root, out result))
            {
                result = new GraphQLExecutionBuilder();
                _executionBuilders.Add(root, result);
            }

            return result;
        }
            
        
    }
}
