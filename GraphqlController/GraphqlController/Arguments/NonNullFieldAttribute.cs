using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class NonNullFieldAttribute : Attribute
    {

    }
}
