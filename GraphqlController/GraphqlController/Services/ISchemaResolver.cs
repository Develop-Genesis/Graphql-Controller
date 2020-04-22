using GraphQL.Types;
using GraphqlController.GraphQl;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Services
{
    public interface ISchemaResolver
    {
        public void BuildSchemas();
        public ISchema GetSchema(Type rootType);       
    }
}
