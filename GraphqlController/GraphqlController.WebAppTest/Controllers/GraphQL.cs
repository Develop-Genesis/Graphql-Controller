using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphqlController.GraphQl;
using GraphqlController.WebAppTest.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GraphqlController.WebAppTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GraphQL : ControllerBase
    {
        IGraphQlTypePool _pool;

        public GraphQL(IGraphQlTypePool pool)
        {
            _pool = pool;
        }

        [HttpPost]
        public async Task<ContentResult> ExecuteQuery([FromBody]Dictionary<string, string> body)
        {           

            var schema = new Schema
            {
                Query = _pool.GetRootGraphType(new Root()) as IObjectGraphType,

            };

            var result = await schema.ExecuteAsync(_ =>
            {
                _.Query = body["query"];
            });
            
            return Content(result, "application/json");
        }
    }
}
