using GraphQL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Cache
{
    public interface IGraphqlInAppCacheService
    {
        Task CacheResponseAsync(string query, string operationName, Inputs variables, object response, CancellationToken cancellationToken);
        Task<object> GetCachedResponseAsync(string query, string operationName, Inputs variables, CancellationToken cancellationToken);
    }
}
