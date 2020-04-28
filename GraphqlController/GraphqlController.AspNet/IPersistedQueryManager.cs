using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore
{
    public interface IPersistedQueryCahce
    {
        Task<bool> IsSupportedAsync(CancellationToken cancellationToken);
        Task AddPersistedQueryAsync(string hash, string query, CancellationToken cancellationToken);
        Task<string> TryGetPersistedQuery(string hash, CancellationToken cancellationToken);
    }
}
