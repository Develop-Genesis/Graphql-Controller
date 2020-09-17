using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApiContrib.Core.Results;

namespace GraphqlController.AspNetCore.Authorization
{
    public class AuthorizationFieldMiddleware
    {        
        public async Task<object> Resolve(
          ResolveFieldContext context,
          FieldMiddlewareDelegate next)
        {
            var serviceProvider = (IServiceProvider)context.UserContext["serviceProvider"];

            var authorizationService = serviceProvider.GetService(typeof(IAuthorizationService)) as IAuthorizationService;
            var httpContext = (serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor)?.HttpContext;

            if(httpContext == null)
            {
                return await next(context);
            }

            object type;
            
            if(context.FieldDefinition.Metadata.TryGetValue("type", out type))
            {

                AuthorizeAttribute fieldAuthorizeAttribute = null;
                AuthorizeAttribute resolveTypeAuthorizeAttribute = null;

                if (type is PropertyInfo propertyInfo)
                {
                    fieldAuthorizeAttribute = propertyInfo.GetCustomAttribute<AuthorizeAttribute>();
                    resolveTypeAuthorizeAttribute = propertyInfo.PropertyType.GetCustomAttribute<AuthorizeAttribute>();
                }
                else if (type is MethodInfo methodInfo)
                {
                    fieldAuthorizeAttribute = methodInfo.GetCustomAttribute<AuthorizeAttribute>();
                    resolveTypeAuthorizeAttribute = methodInfo.ReturnType.GetCustomAttribute<AuthorizeAttribute>();
                }

                if(fieldAuthorizeAttribute != null)
                {
                    if(!await AuthorizeAsync(authorizationService, httpContext, fieldAuthorizeAttribute))
                    {
                        var error = new ExecutionError($"You are not authorize to access field: {context.FieldName}")
                        {
                            Path = context.Path,
                            Code = "401",
                        };
                        error.AddLocation(context.FieldAst.SourceLocation.Line, context.FieldAst.SourceLocation.Column);
                        context.Errors.Add(error);
                        return null;
                    }
                }

                if (resolveTypeAuthorizeAttribute != null)
                {
                    if(!await AuthorizeAsync(authorizationService, httpContext, resolveTypeAuthorizeAttribute))
                    {
                        var error = new ExecutionError($"You are not authorize to access entities of type ${context.ReturnType.Name}")
                        {
                            Path = context.Path,
                            Code = "401",
                        };
                        error.AddLocation(context.FieldAst.SourceLocation.Line, context.FieldAst.SourceLocation.Column);
                        context.Errors.Add(error);
                        return null;
                    }
                }
            }


            return await next(context);
        }

        static async Task<bool> AuthorizeAsync(IAuthorizationService authorizationService, HttpContext httpContext, IAuthorizeData authorizeData)
        {
            var user = httpContext.User;

            AuthorizationResult authorizationResult = null;
            if(authorizeData.Roles != null)
            {
                if(!user.IsInRole(authorizeData.Roles))
                {
                    authorizationResult = AuthorizationResult.Failed();
                }
            }

            if(authorizationResult == null && authorizeData.Policy != null)
            {
                authorizationResult = await authorizationService.AuthorizeAsync(user, authorizeData.Policy);
            }

            if(authorizationResult == null)
            {
                if(user.Identity.IsAuthenticated)
                {                    
                    return true;
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    return false;
                }
            } 
            else
            {
                if(authorizationResult.Succeeded)
                {                    
                    return true;
                }
                else
                {
                    httpContext.Response.StatusCode = 401;
                    return false;
                }                
            }
        }

    }
}
