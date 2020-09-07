using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Class | AttributeTargets.Method)]
    public class NameAttribute : Attribute
    {
        public string Name { get; set; }
        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
