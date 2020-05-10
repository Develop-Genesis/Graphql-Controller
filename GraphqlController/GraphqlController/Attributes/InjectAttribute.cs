using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {
    }
}
