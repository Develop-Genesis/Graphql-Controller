using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class IgnoreFieldAttribute : Attribute
    {
    }
}
