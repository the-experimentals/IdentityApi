using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace IdentityApi.CustomPolicies.AccountPolicy;

public class AccountPolicyHandler : AuthorizationHandler<AccountPolicyRequirement>
{
    private readonly IHttpContextAccessor _httpContext;

    public AccountPolicyHandler(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AccountPolicyRequirement requirement)
    {
        var httpContext = _httpContext.HttpContext;
        var userIdentity = httpContext.User.Identity as ClaimsIdentity;

        if (!httpContext.User.Identity.IsAuthenticated)
        {
            return Task.CompletedTask;
        }

        var role = userIdentity.FindFirst(ClaimTypes.Role).Value;
        var action = httpContext.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>().ActionName;

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
