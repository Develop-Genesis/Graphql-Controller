using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphqlController.AspNetCore
{
    public class GraphQlRequestBody
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        // public object Variables { get; set; }
    }
}
