using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Execution
{
    public class GraphQLResult
    {
        public object Data { get; set; }
        public IEnumerable<GraphQLError> Errors { get; set; }
    }
}
