using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
    public class FieldDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public FieldDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
