using GraphQL;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GraphqlController.AspNet
{
    public class GraphQlResponse
    {
        public object Data { get; set; }

        public ExecutionErrors Errors { get; set; }

        public bool ShouldSerializeErrors()
            => Errors == null;
    }
}
