using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public class DefaultCachePolicy : ICachePolicy
    {
        int MaxAge = -1;
        CacheControlScope Scope = CacheControlScope.Public;

        CacheConfiguration _cacheConfiguration;

        public DefaultCachePolicy(CacheConfiguration cacheConfiguration)
        {
            _cacheConfiguration = cacheConfiguration;
        }

        public void AddCacheControl(CacheControlPayload payload)
        {
            if(payload.MaxAge == -1)
            {
                // ignore unknow cache
                return;
            }

            if (MaxAge == -1)
            {
                MaxAge = payload.MaxAge;                
            }
            else
            {
                MaxAge = Math.Min(MaxAge, payload.MaxAge);
            }
        }

        public int CalculateMaxAge() => MaxAge == -1 ? _cacheConfiguration.DefaultMaxAge : MaxAge;
        
        public CacheControlScope GetScope() => Scope;
    }
}
