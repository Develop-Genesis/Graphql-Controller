using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Arguments
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeNameAttribute : Attribute
    {
        public string Name { get; set; }
        public TypeNameAttribute(string name)
        {
            Name = name;
        }
    }
}
