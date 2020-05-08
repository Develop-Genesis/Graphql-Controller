using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Services
{
    public interface IExecutionBuilderResolver
    {
        IGraphQLExecutionBuilder GetGraphqlExecutionBuilder(Type root);
    }
}
