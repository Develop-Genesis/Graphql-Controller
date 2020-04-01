using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public class NonNullAttribute : Attribute
    {

    }
}
