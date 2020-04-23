using GraphQL.Types;
using GraphqlController.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.GraphQl
{
    public class GraphQlTypePool : IGraphQlTypePool
    {
        // Scalar Type Map
        private Dictionary<Type, IGraphType> ScalarTypeMap = new Dictionary<Type, IGraphType>()
        {
            {  typeof(string), new StringGraphType() },
            {  typeof(int), new IntGraphType() },
            {  typeof(double), new FloatGraphType() },
            {  typeof(bool), new BooleanGraphType() },
        };

        IAssemblyResolver _assemblyResolver;

        private Dictionary<Type, IGraphType> EnumTypeMap = new Dictionary<Type, IGraphType>();
        private Dictionary<Type, IGraphType> ObjectTypeMap = new Dictionary<Type, IGraphType>();
        private Dictionary<Type, IGraphType> UnionTypeMap = new Dictionary<Type, IGraphType>();
        private Dictionary<Type, IGraphType> InterfaceTypeMap = new Dictionary<Type, IGraphType>();

        private Dictionary<string, List<IGraphType>> InterfaceImplementations = new Dictionary<string, List<IGraphType>>();

        private Dictionary<Type, IGraphType> InputObjectTypeMap = new Dictionary<Type, IGraphType>();

        public IGraphType GetRootGraphType(Type rootType)
           => new DynamicGraphType(this, rootType, true);

        public GraphQlTypePool(IAssemblyResolver assemblyResolver)
        {
            _assemblyResolver = assemblyResolver;
        }

        public IGraphType GetGraphType(Type type)
        {
            IGraphType result = null;

            // Check if it is an scalar type
            if (ScalarTypeMap.TryGetValue(type, out result))
                return result;

            // Check if it is a list
            if(typeof(IEnumerable).IsAssignableFrom(type))
            {
                var enumItemType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments()[0];
                return new ListGraphType(GetGraphType(enumItemType));
            }

            // Check if it is a enum
            if(type.IsEnum)
            {
                if (!EnumTypeMap.TryGetValue(type, out result))
                {
                    result = new DynamicEnumerationGraphType(type);
                    EnumTypeMap.Add(type, result);
                }          

                return result;
            }

            // if it is value type and not included in the scalars like Guid
            // use it as string
            if(type.IsValueType)
            {
                return new StringGraphType();
            }

            // Check if it is an interface
            if(type.IsInterface)
            {
                if (!InterfaceTypeMap.TryGetValue(type, out result))
                {
                    result = new DynamicInterfaceType(this, type);
                    InterfaceTypeMap.Add(type, result);
                    RegisterInterfaceTypes(type, result.Name);
                }

                return result;
            }

            // Check if type is a union
            if(typeof(IUnionGraphType).IsAssignableFrom(type))
            {
                if (!UnionTypeMap.TryGetValue(type, out result))
                {
                    var possibleResult = new DynamicUnionGraphType(this, type);

                    // Check again in case it has been added recursevely
                    if (!UnionTypeMap.TryGetValue(type, out result))
                    {
                        result = possibleResult;
                        UnionTypeMap.Add(type, result);
                    }
                }

                return result;
            }

            // Check if it is object
            if(type.IsClass)
            {
                if (!ObjectTypeMap.TryGetValue(type, out result))
                {
                    var possibleResult = new DynamicGraphType(this, type);

                    // Check again in case it has been added recursevely
                    if (!ObjectTypeMap.TryGetValue(type, out result))
                    {
                        result = possibleResult;
                        ObjectTypeMap.Add(type, result);
                    }                        
                }
            }
            
            return result;
        }

        public IGraphType GetInputType(Type type)
        {
            // Check if the type is an scalar type
            IGraphType result = null;

            // Check if it is an scalar type
            if (ScalarTypeMap.TryGetValue(type, out result))
                return result;

            // Check if it is a list
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var enumItemType = type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments()[0];
                return new ListGraphType(GetInputType(enumItemType));
            }

            // Check if it is a enum
            if (type.IsEnum)
            {
                if (!EnumTypeMap.TryGetValue(type, out result))
                {
                    result = new DynamicEnumerationGraphType(type);
                    EnumTypeMap.Add(type, result);
                }
                return result;
            }

            // if it is value type and not included in the scalars like Guid
            // use it as string
            if (type.IsValueType)
            {
                return new StringGraphType();
            }

            // Check if it is object
            if (type.IsClass)
            {
                if (!InputObjectTypeMap.TryGetValue(type, out result))
                {
                    result = new DynamicInputGraphObject(this, type);
                    InputObjectTypeMap.Add(type, result);
                }
            }

            return result;
        }      
        
        private void RegisterInterfaceTypes(Type intrfce, string interfaceName)
        {
            // Get all the types that implement the interface
            var types = _assemblyResolver.GetAssemblies().SelectMany(assembly => assembly.GetTypes()
                                                                .Where(type => intrfce.IsAssignableFrom(type)));

            // Add all the types that implement the interface to the pool
            foreach(var type in types)
            {
                List<IGraphType> implementors;

                if(!InterfaceImplementations.TryGetValue(interfaceName, out implementors))
                {
                    implementors = new List<IGraphType>();
                    InterfaceImplementations.Add(interfaceName, implementors);
                }

                implementors.Add(GetGraphType(type));                
            }
        }

        public IEnumerable<IGraphType> GetInterfaceImplementations(string interfaceName)
        {
            return InterfaceImplementations[interfaceName];
        }
    }
}
