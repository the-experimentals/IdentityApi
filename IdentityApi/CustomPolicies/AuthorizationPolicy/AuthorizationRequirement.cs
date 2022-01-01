using System;
using Microsoft.AspNetCore.Authorization;

namespace IdentityApi.CustomPolicies.AuthorizationPolicy
{
	public class AuthorizationRequirement : IAuthorizationRequirement
	{
		public AuthorizationRequirement()
		{
		}
	}
}

