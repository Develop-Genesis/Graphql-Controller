using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    public class CustomGraphTypeAttribute : Attribute
    {
        public Type Type { get; }

        public string TypeName { get; }

        public CustomGraphTypeAttribute(Type type)
        {
            Type = type;
        }

        public CustomGraphTypeAttribute(string typeName)
        {
            TypeName = typeName;
        }

    }
}
