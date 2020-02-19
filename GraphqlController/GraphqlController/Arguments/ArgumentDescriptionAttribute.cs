using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public ArgumentDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
