using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    public class SubscriptionAttribute : Attribute
    {
        public Type Root { get; }

        public SubscriptionAttribute(Type root)
        {
            Root = root;
        }
    }
}
