using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MutationAttribute : Attribute
    {
        public Type Root { get; }
        public MutationAttribute(Type root)
        {
            Root = root;
        }
    }
}
