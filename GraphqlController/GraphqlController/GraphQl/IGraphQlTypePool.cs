using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.GraphQl
{
    public interface IGraphQlTypePool
    {
        IEnumerable<IGraphType> GetInterfaceImplementations(string interfaceName);
        IGraphType GetGraphType(Type type);
        IGraphType GetInputType(Type type);
        IGraphType GetRootGraphType(Type rootType);
        IGraphType GetSubscriptionType(IEnumerable<Type> subscriptionTypes);
        IGraphType GetMutationType(IEnumerable<Type> mutationTypes);
    }
}
