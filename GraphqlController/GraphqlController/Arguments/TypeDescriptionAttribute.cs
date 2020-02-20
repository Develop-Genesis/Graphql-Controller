using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public TypeDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
