using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.Services
{
    public interface ICustomScalarTypesResolver
    {
        public IEnumerable<KeyValuePair<Type, IGraphType>> GetCustomScalars();
    }

    public class CustomScalarTypesResolver : ICustomScalarTypesResolver
    {
        List<KeyValuePair<Type, IGraphType>> _types = new List<KeyValuePair<Type, IGraphType>>(); 

        public CustomScalarTypesResolver()
        {

        }

        public void AddScalarType(Type type, IGraphType graphType)
        {
            _types.Add(new KeyValuePair<Type, IGraphType>(type, graphType));
        }

        public IEnumerable<KeyValuePair<Type, IGraphType>> GetCustomScalars()
        {
            return _types.Select(x => x);
        }
    }

}
