using GraphQL;
using GraphQL.Execution;
using GraphqlController.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Cache
{
    public class GraphqlInAppCacheService : IGraphqlInAppCacheService
    {
        CacheConfiguration _cacheConfiguration;
        ICachePolicy _cachePolicy;
        IDistributedCache _distributedCache;
        IMemoryCache _memoryCache;

        const string CachePrefix = "GraphqalResponseCache_";

        public GraphqlInAppCacheService(
            CacheConfiguration cacheConfiguration, 
            ICachePolicy cachePolicy,
            IScopedServiceProviderResolver serviceProviderResolver)
        {
            _cacheConfiguration = cacheConfiguration;
            _cachePolicy = cachePolicy;

            switch(cacheConfiguration.ResponseCache)
            {
                case ResponseCacheType.Memory:
                    _memoryCache = serviceProviderResolver.GetProvider().GetService<IMemoryCache>();
                    break;

                case ResponseCacheType.Distributed:
                    _distributedCache = serviceProviderResolver.GetProvider().GetService<IDistributedCache>();
                    break;                    
            }
        }

        public async Task CacheResponseAsync(string query, string operationName, Inputs variables, object response, CancellationToken cancellationToken)
        {
            if(_cacheConfiguration.ResponseCache == ResponseCacheType.None || response == null)
            {
                return;
            }

            var key = GenerateKey(query, operationName, variables);

            var maxAge = _cachePolicy.CalculateMaxAge();
            var scope = _cachePolicy.GetScope();

            switch (_cacheConfiguration.ResponseCache)
            {
                case ResponseCacheType.Memory:
                    _memoryCache.Set(key, response);
                    break;

                case ResponseCacheType.Distributed:

                    using(var ms = new MemoryStream())
                    {
                        using(var writer = new BsonWriter(ms))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(writer, response);

                            await _distributedCache.SetAsync(
                                key, 
                                ms.ToArray(), 
                                new DistributedCacheEntryOptions() { 
                                   AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(maxAge)                                  
                                }, 
                                cancellationToken);
                        }
                    }
                    
                    break;
            }

        }

        public async Task<object> GetCachedResponseAsync(string query, string operationName, Inputs variables, CancellationToken cancellationToken)
        {
            if (_cacheConfiguration.ResponseCache == ResponseCacheType.None)
            {
                return null;
            }

            var key = GenerateKey(query, operationName, variables);

            switch (_cacheConfiguration.ResponseCache)
            {
                case ResponseCacheType.Memory:
                    object result;
                    _memoryCache.TryGetValue(key, out result);
                    return result;
                    
                case ResponseCacheType.Distributed:
                    var bytes = await _distributedCache.GetAsync(key, cancellationToken);

                    if(bytes == null)
                    {
                        return null;
                    }

                    using(var ms = new MemoryStream(bytes))
                    {
                        using (BsonReader reader = new BsonReader(ms))
                        {
                            JsonSerializer serializer = new JsonSerializer();

                            var obj = serializer.Deserialize(reader);
                            return ConvertJsonResultToNetTypes(obj);
                        }
                    }
            }

            return null;

        }

        static string GenerateKey(string query, string operationName, Inputs variables)
            => CachePrefix + Helpers.GetSha256Hash(query + operationName + JsonConvert.SerializeObject(variables));

        static object ConvertJsonResultToNetTypes(object obj)
        {
            return obj switch
            {
                JObject jObject => ConvertJObjectToNetType(jObject),
                JArray jArray => ConvertJArrayToNetType(jArray),
                JValue val => val.Value,
                _ => obj
            };
        }

        static object ConvertJArrayToNetType(JArray jArray)
          => jArray.Select(x => ConvertJsonResultToNetTypes(x));

        static object ConvertJObjectToNetType(JObject jObject)
          => new Dictionary<string, object>(
              (jObject as IEnumerable<KeyValuePair<string, JToken>>)
              .Select(x => new KeyValuePair<string, object>(x.Key, ConvertJsonResultToNetTypes(x.Value)))
              );

    }
}
