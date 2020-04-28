using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class InMemoryPersistedQueryCache : IPersistedQueryCahce
    {
        Dictionary<string, string> _cache = new Dictionary<string, string>();

        public Task AddPersistedQueryAsync(string hash, string query, CancellationToken cancellationToken)
        {
            _cache.Add(hash, query);
            return Task.CompletedTask;
        }

        public Task<bool> IsSupportedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task<string> TryGetPersistedQuery(string hash, CancellationToken cancellationToken)
        {
            string value;
            if(_cache.TryGetValue(hash, out value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult<string>(null);
        }
    }
}
