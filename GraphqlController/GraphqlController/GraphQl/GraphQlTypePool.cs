using GraphQL.Types;
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
        private Dictionary<Type, IGraphType> EnumTypeMap = new Dictionary<Type, IGraphType>();
        private Dictionary<Type, IGraphType> ObjectTypeMap = new Dictionary<Type, IGraphType>();

        public IGraphType GetGraphType(Type type)
        {
            IGraphType result = null;

            // Check if it is an scalar type
            if (ScalarTypeMap.TryGetValue(type, out result))
                return result;

            // Check if it is a list
            if(typeof(IEnumerable).IsAssignableFrom(type))
            {
                var enumItemType = type.GetInterfaces().First(x => x.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GetGenericArguments()[0];
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

            // Check if it is object
            if(typeof(IGraphNodeType).IsAssignableFrom(type))
            {
                if (!ObjectTypeMap.TryGetValue(type, out result))
                {
                    result = new DynamicGraphType(this, type);
                    ObjectTypeMap.Add(type, result);
                }
            }
            
            return result;
        }

        public IGraphType GetInputType(Type type)
        {
            // throw new NotImplementedException();
            return GetGraphType(type);
        }        
    }
}
