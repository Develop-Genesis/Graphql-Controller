using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphqlController.GraphQl;
using GraphqlController.Services;
using GraphqlController.WebAppTest.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphqlController.WebAppTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GraphQL : ControllerBase
    {
        ISchema _schema;
        IScopedServiceProviderResolver _scopedServiceProviderResolver;

        public GraphQL(ISchema schema, IScopedServiceProviderResolver scopedServiceProviderResolver)
        {
            _schema = schema;
            _scopedServiceProviderResolver = scopedServiceProviderResolver;
        }

        [HttpPost]
        public async Task<ContentResult> ExecuteQuery([FromBody]Dictionary<string, string> body)
        {           

            var result = await _schema.ExecuteAsync(_ =>
            {
                _.Query = body["query"];
                _.UserContext = new Dictionary<string, object>() { { "serviceProvider", _scopedServiceProviderResolver.GetProvider() } };
            });
            
            return Content(result, "application/json");
        }
    }
}
