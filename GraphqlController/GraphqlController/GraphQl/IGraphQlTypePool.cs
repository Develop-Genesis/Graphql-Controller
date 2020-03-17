using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.GraphQl
{
    public interface IGraphQlTypePool
    {
        IGraphType GetGraphType(Type type);
        IGraphType GetInputType(Type type);
        IGraphType GetRootGraphType(Type rootType);        
    }
}
