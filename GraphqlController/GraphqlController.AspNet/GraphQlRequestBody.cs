using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphqlController.AspNetCore
{
    public class GraphQlRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public JObject Variables { get; set; }
        public GraphqlExtensions Extensions { get; set; }        
    }

    public class GraphqlExtensions
    {
        public GraphqlPersistedQuery PersistedQuery { get; set; }
    }

    public class GraphqlPersistedQuery
    {
        public int Version { get; set; }
        public string Sha256Hash { get; set; }
    }
}
