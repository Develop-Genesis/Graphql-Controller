using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphqlController.WebAppTest.Types
{
    public class CustomType : ObjectGraphType
    {
        public CustomType()
        {
            Field(typeof(StringGraphType), "name", resolve: (c) => "Alejandro");
            Field(typeof(StringGraphType), "lastName", resolve: (c) => "Guardiola");                  
        }
    }
}
