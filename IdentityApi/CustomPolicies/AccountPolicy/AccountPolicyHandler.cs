using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
            string role = userIdentity.FindFirst(ClaimTypes.Role).Value;
            PathString path = httpContext.Request.Path;

            if (role.Equals("ADMIN"))
            {
                context.Succeed(requirement);
            }
            else
            {

                // check for path here and then decaide.

                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}

