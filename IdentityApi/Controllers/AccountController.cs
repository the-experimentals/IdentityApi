using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Mappings;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PwnedPasswords.Client;

namespace IdentityApi.Controllers
{
    [Authorize]
    [Route(AccountMappings.ENDPOINT_ROUTE)]
    public class AccountController : Controller
    {
        private readonly IAccountManager _accountManager;
        private readonly IPwnedPasswordsClient _pwnedPasswords;

        public AccountController(IAccountManager accountManager, IPwnedPasswordsClient pwnedPasswords)
        {
            _accountManager = accountManager;
            _pwnedPasswords = pwnedPasswords;
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Testing account endpoint");
        }

        /// <summary>
        /// Endpoing action for registering new profile request.
        /// </summary>
        /// <param name="newProfileView">Request model for containing new profile data.</param>
        /// <returns>Boolean true for successful profile creation.</returns>
        /// <response code="201">User's credentials have been authenticated successfully</response>
        /// <response code="401">Unsuccessfull user's authentication, check errors in response model.</response> 
        [HttpPost(AccountMappings.CREATE_NEW_PROFILE)]
        [ProducesResponseType(typeof(NewProfileRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public IActionResult CreateNewProfile(NewProfileRequest newProfileView)
        {
            NewProfileResponse profileResponse = new();

            ClaimsIdentity userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string profileID = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            Profile userProfile = new()
            {
                NAME = string.Concat(newProfileView.FIRST_NAME.Trim(), " ", newProfileView.LAST_NAME.Trim()),
                EMAIL = newProfileView.EMAIL.Trim().ToLower(),
                NEW = true,
                CREATED_BY = profileID
            };

            userProfile.CREDENTIAL = new()
            {
                USERNAME = newProfileView.USERNAME.Trim().ToLower(),
                PASSWORD = newProfileView.PASSWORD
            };

            ProfileSaveStatus result = _accountManager.CreateProfile(userProfile);

            if (result.IS_SAVED)
            {
                profileResponse.IS_SAVED = true;

                return Created(AccountMappings.CREATE_NEW_PROFILE,profileResponse);
            }
            else
                return BadRequest(result.ERRORS);
        }
    }
}
