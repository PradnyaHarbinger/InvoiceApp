using InvoiceApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace InvoiceApp.Authorization
{
    public class InvoiceCreatorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Invoice> 
    {
        UserManager<IdentityUser> _userManager;  //we preceed private variables using underscore_
        public InvoiceCreatorAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Invoice invoice)
        {
            // whatever authorization we have we can now implement it here

            // 1. - User is present or invoice is present if not
            if(context.User == null || invoice == null)
            {
                return Task.CompletedTask;
            }

            // 2. - User want to do exactly a crud operation
            if(requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // this is the only real succeed state that we need
            // context.user -> is current logged in user - if its id is same as invoice creatorId then only he/she will be able to perform RUD operation
            if(invoice.CreatorId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            // to return if the authorization fail
            return Task.CompletedTask;    


            // if the authorization succeeds
            //context.Succeed(requirement);
        }
    }
}
