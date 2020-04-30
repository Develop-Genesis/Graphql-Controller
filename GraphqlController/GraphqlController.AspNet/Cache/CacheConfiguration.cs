using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Cache
{
    public class CacheConfiguration
    {
        public int DefaultMaxAge { get; set; } = 0;
        public bool UseHttpCaching { get; set; } = false;
        public bool IncludeETag { get; set; } = false;
        public ResponseCacheType ResponseCache { get; set; } = ResponseCacheType.None;
    }

    public enum ResponseCacheType
    {
        None,
        Memory,
        Distributed
    }

}
