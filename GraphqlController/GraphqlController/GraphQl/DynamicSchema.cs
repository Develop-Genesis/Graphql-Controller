using GraphQL.Types;
using GraphqlController.Attributes;
using GraphqlController.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class DynamicSchema : Schema
    {
        public DynamicSchema(IGraphQlTypePool pool, Type query, IEnumerable<Type> mutationTypes, IEnumerable<Type> subscriptionTypes)
        {           
            if(query == null)
            {
                throw new InvalidOperationException("Cannot find Root type, make sure that it is a valid GraphNodeType and has RootType attribute");
            }

            var graphqlRootType = pool.GetRootGraphType(query);
            var subscriptionGraphType = pool.GetSubscriptionType(subscriptionTypes);
            var mutationGraphType = pool.GetMutationType(mutationTypes);

            Query = graphqlRootType as IObjectGraphType;
            Mutation = mutationGraphType as IObjectGraphType;
            Subscription = subscriptionGraphType as IObjectGraphType;

            var allInterfaces = Query.GetAllInterfaces()
                                     .Concat(Mutation.GetAllInterfaces())
                                     .Concat(Subscription.GetAllInterfaces())
                                     .Distinct();

            foreach(var intrface in allInterfaces)
            {
                var implementations = pool.GetInterfaceImplementations(intrface.Name);
                RegisterTypes(implementations.ToArray());
            }
        }



    }
}
