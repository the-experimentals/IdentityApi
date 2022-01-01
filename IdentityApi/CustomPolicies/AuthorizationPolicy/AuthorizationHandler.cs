using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace IdentityApi.CustomPolicies.AuthorizationPolicy
{
	public class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
	{
		public AuthorizationHandler()
		{
		}

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
        {

            return Task.CompletedTask;
        }
    }
}

