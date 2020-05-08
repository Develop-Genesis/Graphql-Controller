using GraphQL.Conversion;
using GraphqlController.Execution;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.PersistedQuery
{
    public class PersistedQueryService : IPersistedQueryService
    {
        IPersistedQueryCache _persistedQueryManager;

        public PersistedQueryService(IPersistedQueryCache persistedQueryManager)
        {
            _persistedQueryManager = persistedQueryManager;
        }

        public async Task<RegisterPersistedQueryResult> TryRegisterQueryAsync(GraphQlRequest request, CancellationToken cancellationToken)
        {            
            if (request.Query == null)
            {
                if (!await _persistedQueryManager.IsSupportedAsync(cancellationToken))
                {
                    return RegisterPersistedQueryResult.PersistedQueryNotSupported;
                }
                return RegisterPersistedQueryResult.NoQueryToRegister;
            }
                        
            if (request.Extensions?.GetValue("persistedQuery") == null)
            {
                return RegisterPersistedQueryResult.NoPersistedQuery;
            }

            if (!await _persistedQueryManager.IsSupportedAsync(cancellationToken))
            {
                return RegisterPersistedQueryResult.PersistedQueryNotSupported;
            }

            // Validate hash
            if ((string)request.Extensions["persistedQuery"]["sha256Hash"] != Helpers.GetSha256Hash(request.Query))
            {
                return RegisterPersistedQueryResult.PersistedQueryNotSupported;
            }

            await _persistedQueryManager.AddPersistedQueryAsync((string)request.Extensions["persistedQuery"]["sha256Hash"], request.Query, cancellationToken);

            return RegisterPersistedQueryResult.QueryRegistered;
        }


        public async Task<string> GetPersistedQueryAsync(string sha256, CancellationToken cancellationToken)
        {
            var query = await _persistedQueryManager.TryGetPersistedQuery(sha256, cancellationToken);
            return query;
        }

    }
}
