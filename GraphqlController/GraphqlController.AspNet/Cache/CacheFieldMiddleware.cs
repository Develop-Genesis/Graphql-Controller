using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphqlController.AspNetCore.Cache
{
    public class CacheFieldMiddleware
    {
        public async Task<object> Resolve(
          ResolveFieldContext context,
          FieldMiddlewareDelegate next)
        {
            var _cachePolicy = (ICachePolicy)((IServiceProvider)context.UserContext["serviceProvider"]).GetService(typeof(ICachePolicy));

            CacheControlPayload cacheControlPayload = null;

            // check if the field contains cache attribute
            object tempValue;
            if(context.FieldDefinition.Metadata.TryGetValue(CacheControlAttribute.MetadataKey, out tempValue))
            {
                cacheControlPayload = tempValue as CacheControlPayload;             
            }
            // check if the field type has cache attriubute
            else 
            {
                IGraphType returnType;
                if(context.ReturnType is ListGraphType list)
                {
                    returnType = list.ResolvedType;
                }
                else
                {
                    returnType = context.ReturnType;
                }
                if (returnType.Metadata.TryGetValue(CacheControlAttribute.MetadataKey, out tempValue))
                {
                    cacheControlPayload = tempValue as CacheControlPayload;
                }                    
            }
            
            if(cacheControlPayload == null)
            {
                // if the cache is not assigned by the user use the default
                cacheControlPayload = new CacheControlPayload(-1, CacheControlScope.Public);
            }

            // add the cache payload to the cache policy
            _cachePolicy.AddCacheControl(cacheControlPayload);

            return await next(context);
            
        }
    }
}
