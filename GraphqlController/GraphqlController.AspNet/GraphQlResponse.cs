﻿using GraphQL;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GraphqlController.AspNetCore
{  

    public class GraphQLError
    {
        public string Message { get; set; }
        public IEnumerable<ErrorLocation> Locations { get; set; }
        public IEnumerable<object> Path { get; set; }
        public Dictionary<string, object> Extensions { get; set; }
    }

    public class ErrorLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }
    }

}
