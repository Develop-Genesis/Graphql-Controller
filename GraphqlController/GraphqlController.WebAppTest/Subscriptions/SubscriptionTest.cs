using GraphqlController.Attributes;
using GraphqlController.WebAppTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Subscriptions
{
    [Subscription(typeof(Root))]
    public class SubscriptionTest : GraphNodeType
    {
        public IObservable<Teacher> AllPersons => new Teacher[] {

            new Teacher()
            {
                Name = "Alejo",
                LastName = "Guardiola",
            },
            new Teacher()
            {
                Name = "AlejoA",
                LastName = "GuardiolaA",
            },
            new Teacher()
            {
                Name = "AlejoB",
                LastName = "GuardiolaB",
            },
            new Teacher()
            {
                Name = "AlejoC",
                LastName = "GuardiolaC",
            },
            new Teacher()
            {
                Name = "AlejoD",
                LastName = "GuardiolaD",
            }

        }.ToObservable().Zip(Observable.Interval(TimeSpan.FromSeconds(2)), (x, y) => x);
    }
}
