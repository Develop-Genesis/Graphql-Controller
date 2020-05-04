using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.Execution
{
    public class GraphQlRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public JObject Variables { get; set; }
        public JObject Extensions { get; set; }
    }
}
