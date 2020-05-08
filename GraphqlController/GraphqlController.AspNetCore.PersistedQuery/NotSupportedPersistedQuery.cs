using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class NotSupportedPersistedQuery : IPersistedQueryCache
    {
        public Task AddPersistedQueryAsync(string hash, string query, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSupportedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task<string> TryGetPersistedQuery(string hash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
