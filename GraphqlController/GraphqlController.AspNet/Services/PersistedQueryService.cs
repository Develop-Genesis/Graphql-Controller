using GraphQL.Conversion;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Services
{
    public class PersistedQueryService : IPersistedQueryService
    {
        IPersistedQueryCahce _persistedQueryManager;

        public PersistedQueryService(IPersistedQueryCahce persistedQueryManager)
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

            if (request.Extensions?.PersistedQuery == null)
            {
                return RegisterPersistedQueryResult.NoPersistedQuery;
            }

            if (!await _persistedQueryManager.IsSupportedAsync(cancellationToken))
            {
                return RegisterPersistedQueryResult.PersistedQueryNotSupported;
            }

            // Validate hash
            if (request.Extensions.PersistedQuery.Sha256Hash != GetSha256Hash(request.Query))
            {
                return RegisterPersistedQueryResult.PersistedQueryNotSupported;
            }

            await _persistedQueryManager.AddPersistedQueryAsync(request.Extensions.PersistedQuery.Sha256Hash, request.Query, cancellationToken);

            return RegisterPersistedQueryResult.QueryRegistered;
        }


        public async Task<string> GetPersistedQueryAsync(string sha256, CancellationToken cancellationToken)
        {
            var query = await _persistedQueryManager.TryGetPersistedQuery(sha256, cancellationToken);
            return query;
        }

        private static string GetSha256Hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

    }
}
