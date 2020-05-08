using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public interface ICachePolicy
    {
        void AddCacheControl(CacheControlPayload payload);
        int CalculateMaxAge();
        CacheControlScope GetScope();
    }
}
