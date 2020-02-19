using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NonNullArgumentAttribute : Attribute
    {

    }
}
