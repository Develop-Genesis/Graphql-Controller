using GraphqlController.Execution;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public static class ExecutionDataDictionaryExtensions
    {
        public static bool IsHttpRequest(this IExecutionDataDictionary data)
        {
            return data.ContainsKey("IsHttpRequest");
        }

        public static HttpContext GetHttpContext(this IExecutionDataDictionary data)
        {
            return data["HttpContext"] as HttpContext;
        }
    }
}
