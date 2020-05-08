using GraphqlController.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public interface IPersistedQueryService
    {
        Task<RegisterPersistedQueryResult> TryRegisterQueryAsync(GraphQlRequest request, CancellationToken cancellationToken);
        Task<string> GetPersistedQueryAsync(string sha256, CancellationToken cancellationToken);
    }

    public enum RegisterPersistedQueryResult
    {
        QueryRegistered,
        NoQueryToRegister,
        PersistedQueryNotSupported,
        NoPersistedQuery
    }

}
