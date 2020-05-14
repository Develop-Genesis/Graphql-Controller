using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Subscriptions.Dtos
{
    public class OperationMessage
    {
        [JsonProperty("payload")]
        public JToken Payload { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
