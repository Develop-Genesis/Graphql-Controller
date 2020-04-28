using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class InMemoryPersistedQueryCache : IPersistedQueryCahce
    {
        IMemoryCache _memoryCache;

        public InMemoryPersistedQueryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task AddPersistedQueryAsync(string hash, string query, CancellationToken cancellationToken)
        {
            _memoryCache.Set(hash, query);            
            return Task.CompletedTask;
        }

        public Task<bool> IsSupportedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> TryGetPersistedQuery(string hash, CancellationToken cancellationToken)
        {
            string value;
            if(_memoryCache.TryGetValue(hash, out value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult<string>(null);
        }
    }
}
