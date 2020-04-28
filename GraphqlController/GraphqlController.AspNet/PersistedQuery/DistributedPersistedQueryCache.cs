using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class DistributedPersistedQueryCache : IPersistedQueryCahce
    {
        IDistributedCache _distributeCache;

        public DistributedPersistedQueryCache(IDistributedCache distributeCache)
        {
            _distributeCache = distributeCache;
        }

        public Task AddPersistedQueryAsync(string hash, string query, CancellationToken cancellationToken)
          => _distributeCache.SetAsync(hash, Encoding.UTF8.GetBytes(query), cancellationToken);
        

        public Task<bool> IsSupportedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_distributeCache != null);
        }

        public async Task<string> TryGetPersistedQuery(string hash, CancellationToken cancellationToken)
        {
            try
            {
                var bytes = await _distributeCache.GetAsync(hash, cancellationToken);
                if(bytes == null || bytes.Length == 0)
                {
                    return null;
                }

                return Encoding.UTF8.GetString(bytes);
            } catch
            {
                return null;
            }
              
        }
    }
}
