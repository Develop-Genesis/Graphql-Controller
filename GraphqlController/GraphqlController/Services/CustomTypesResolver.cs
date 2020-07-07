using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Services
{
    public class CustomTypesResolver : ICustomTypesResolver
    {
        List<IGraphType> CustomTypes;

        public CustomTypesResolver()
        {
            CustomTypes = new List<IGraphType>();
        }

        public void AddCustomType(IGraphType graphType)
            => CustomTypes.Add(graphType);

        public IEnumerable<IGraphType> GetCustomTypes()
            => CustomTypes;
    }
}
