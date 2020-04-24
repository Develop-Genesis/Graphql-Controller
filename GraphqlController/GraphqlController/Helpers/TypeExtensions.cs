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

        public static IEnumerable<Type> GetInterfacesIncludingType(this Type type)
        {
            if (type.IsInterface)
                yield return type;

            foreach(var intrfce in type.GetInterfaces())
            {
                yield return intrfce;
            }
        }

        // I liked this code, from https://stackoverflow.com/questions/52758745/get-interfaces-implemented-by-class
        public static IEnumerable<Type> GetNotDerivedInterfaces(this Type type)
        {
            //From your question, I think that you only want to handle
            //class case so I am throwing here but you can handle accordingly
            if (type.IsClass == false)
            {
                throw new NotSupportedException($"{type} must be a class but it is not!");
            }

            //All of the interfaces implemented by the class
            var allInterfaces = new HashSet<Type>(type.GetInterfaces());

            //Type one step down the hierarchy 
            var baseType = type.BaseType;

            //If it is not null, it might implement some other interfaces
            if (baseType != null)
            {
                //So let us remove all the interfaces implemented by the base class
                allInterfaces.ExceptWith(baseType.GetInterfaces());
            }

            //NOTE: allInterfaces now only includes interfaces implemented by the most derived class and
            //interfaces implemented by those(interfaces of the most derived class)

            //We want to remove interfaces that are implemented by other interfaces
            //i.e
            //public interface A : B{}
            //public interface B {}
            //public class Top : A{}→ We only want to dump interface A so interface B must be removed

            var toRemove = new HashSet<Type>();
            //Considering class A given above allInterfaces contain A and B now
            foreach (var implementedByMostDerivedClass in allInterfaces)
            {
                //For interface A this will only contain single element, namely B
                //For interface B this will an empty array
                foreach (var implementedByOtherInterfaces in implementedByMostDerivedClass.GetInterfaces())
                {
                    toRemove.Add(implementedByOtherInterfaces);
                }
            }

            //Finally remove the interfaces that do not belong to the most derived class.
            allInterfaces.ExceptWith(toRemove);

            //Result
            return allInterfaces;
        }

    }
}
