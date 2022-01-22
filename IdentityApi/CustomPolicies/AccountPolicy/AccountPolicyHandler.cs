using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityApi.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IdentityApi.CustomPolicies.AccountPolicy
{
	public class AccountPolicyHandler : AuthorizationHandler<AccountPolicyRequirement>
    {
		IHttpContextAccessor _httpContext;
		public AccountPolicyHandler(IHttpContextAccessor httpContext)
		{
			_httpContext = httpContext;
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountPolicyRequirement requirement)
        {
            HttpContext httpContext = _httpContext.HttpContext;
            ClaimsIdentity userIdentity = httpContext.User.Identity as ClaimsIdentity;

            if (!httpContext.User.Identity.IsAuthenticated)
                return Task.CompletedTask;

            string role = userIdentity.FindFirst(ClaimTypes.Role).Value;
            string action = httpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>().ActionName;

            if (role.Equals("ADMIN"))
            {
                context.Succeed(requirement);
            }
            else
            {
                switch (action)
                {
                    case "GetProfileView":
                        context.Succeed(requirement);
                        break;
                    default:
                        context.Fail();
                        break;
                }
                //check for path here and then decaide.

                
            }


            return Task.CompletedTask;
        }
    }
}

