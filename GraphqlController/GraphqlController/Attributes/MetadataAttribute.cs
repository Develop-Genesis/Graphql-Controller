using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class MetadataAttribute : Attribute
    {
        public string Key { get; private set; }

        public object Value { get; private set; }

        public MetadataAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

    }
}
