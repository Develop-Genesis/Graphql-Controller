using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GraphqlController.Helpers
{
    public static class TypeExtensions
    {
        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            var attrType = typeof(T);
            var instance = Attribute.GetCustomAttribute(type, attrType);
            if (instance == null)
                return null;
            return instance as T;
        }

        public static T GetAttribute<T>(this PropertyInfo type) where T : Attribute
        {
            var attrType = typeof(T);
            var instance = Attribute.GetCustomAttribute(type, attrType);
            if (instance == null)
                return null;
            return instance as T;
        }

        public static T GetAttribute<T>(this MethodInfo type) where T : Attribute
        {
            var attrType = typeof(T);
            var instance = Attribute.GetCustomAttribute(type, attrType);
            if (instance == null)
                return null;
            return instance as T;
        }


        public static T GetAttribute<T>(this ParameterInfo type) where T : Attribute
        {
            var attrType = typeof(T);
            var instance = Attribute.GetCustomAttribute(type, attrType);
            if (instance == null)
                return null;
            return instance as T;
        }
    }
}
