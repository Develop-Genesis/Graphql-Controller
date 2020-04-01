using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentNameAttribute : Attribute
    {
        public string Name { get; set; }
        public ArgumentNameAttribute(string name)
        {
            Name = name;
        }
    }
}
