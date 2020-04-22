﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GraphqlController.AspNet
{
    public class GraphQlRequestBody
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public string Variables { get; set; }
    }
}
