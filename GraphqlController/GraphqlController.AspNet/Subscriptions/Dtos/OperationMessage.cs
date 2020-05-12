using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Subscriptions.Dtos
{
    public class OperationMessage
    {
        public JValue Payload { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
    }
}
