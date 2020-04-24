using GraphqlController.Attributes;
using GraphqlController.WebAppTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Subscriptions
{
    [Subscription(typeof(Root))]
    public class SubscriptionTest : GraphNodeType
    {
        public IObservable<IPerson> AllPersons => Observable.Empty<IPerson>();
    }
}
