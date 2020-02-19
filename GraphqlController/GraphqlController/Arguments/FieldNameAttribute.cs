using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class FieldNameAttribute : Attribute
    {
        public string Name { get; set; }
        public FieldNameAttribute(string name)
        {
            Name = name;
        }
    }
}
