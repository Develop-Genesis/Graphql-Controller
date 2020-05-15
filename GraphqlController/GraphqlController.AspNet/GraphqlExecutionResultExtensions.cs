using GraphQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphqlController.AspNetCore
{
    public static class GraphqlExecutionResultExtensions
    {
        public static Dictionary<string, object> ToResultDictionary(this ExecutionResult result)
        {
            var dictionary = new Dictionary<string, object>();

            dictionary["data"] = result.Data;

            if (result.Errors != null)
            {
                dictionary["errors"] = result.Errors.Select(error => new GraphQLError()
                {
                    Message = error.Message,
                    Path = error.Path,
                    Locations = error.Locations?.Select(loc => new ErrorLocation
                    {
                        Column = loc.Column,
                        Line = loc.Line
                    }),
                    Extensions = error.DataAsDictionary
                });
            }

            return dictionary;
        }
    }
}
