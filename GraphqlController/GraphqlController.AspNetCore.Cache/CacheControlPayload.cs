using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public class CacheControlPayload
    {
        public CacheControlPayload(int maxAge, CacheControlScope scope)
        {
            MaxAge = maxAge;
            Scope = scope;
        }

        public int MaxAge { get; }
        public CacheControlScope Scope { get; }
    }

    public enum CacheControlScope
    {
        Public,
        Private
    }


    public static class CacheControlScopeExtensions
    {
        public static string ToHttpHeader(this CacheControlScope responseCacheType)
            => responseCacheType == CacheControlScope.Public ? "public" : "private";
    }


}
