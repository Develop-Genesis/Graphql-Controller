using GraphQL.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GraphqlController.Authorization
{
    public class AuthorizationValidationRule : IValidationRule
    {
        public Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var serviceProvider = context.UserContext["serviceProvider"] as IServiceProvider;



            throw new NotImplementedException();
        }
    }
}
