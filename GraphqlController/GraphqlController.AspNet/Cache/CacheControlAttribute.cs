using GraphqlController.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public class CacheControlAttribute : MetadataAttribute
    {
        public const string MetadataKey = "CacheControl";
        public CacheControlAttribute(int maxAge, CacheControlScope scope = CacheControlScope.Public) : base(MetadataKey, new CacheControlPayload(maxAge, scope))
        {

        }
    }

}
