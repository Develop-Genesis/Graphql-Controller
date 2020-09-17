using System;
using System.Collections.Generic;
using System.Text;

namespace GraphqlController.AspNetCore.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class AuthorizeAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute
    {
        public AuthorizeAttribute() { }

        public AuthorizeAttribute(string policy) : base(policy) { }        
    }
}
