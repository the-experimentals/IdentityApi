using System;
using Microsoft.AspNetCore.Authorization;

namespace IdentityApi.CustomPolicies.AccountPolicy
{
	public class AccountPolicyRequirement : IAuthorizationRequirement
	{
		public AccountPolicyRequirement()
		{
		}
	}
}

