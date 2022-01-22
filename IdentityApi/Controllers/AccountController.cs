using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.EmailTemplate;
using IdentityApi.Mappings;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.CustomRazorEngine;
using IdentityApi.Services.gRPC.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PwnedPasswords.Client;

namespace IdentityApi.Controllers
{
    [Authorize(Policy = "AccountPolicy")]
    [ApiController]
    [Route(AccountMappings.ENDPOINT_ROUTE)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AccountController : Controller
    {
        private readonly IAccountManager _accountManager;
        private readonly IPwnedPasswordsClient _pwnedPasswords;
        private readonly NotificationClient _notificationClient;
        private readonly ICustomRazorEngine _customRazorEngine;

        public AccountController(IAccountManager accountManager, IPwnedPasswordsClient pwnedPasswords, NotificationClient notificationClient, ICustomRazorEngine customRazorEngine)
        {
            _accountManager = accountManager;
            _pwnedPasswords = pwnedPasswords;
            _notificationClient = notificationClient;
            _customRazorEngine = customRazorEngine;
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
        public IActionResult CreateNewProfile([FromBody]NewProfileRequest newProfileView)
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

        /// <summary>
        /// Endpoint action for accepting client's request to check password against pwned password list.
        /// </summary>
        /// <param name="passwordRequest">Password to be checked against pwned password list.</param>
        /// <returns>Retruns HTTP Status 200 along with password matched result and 400 if massword is missing from request query.</returns>
        /// <response code="200">Password match found in pwned password list or not.</response>
        /// <response code="400">Password missing from request query.</response> 
        [HttpGet(AccountMappings.CHECK_PWNED_PASSWORD)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckPwnedPasswordAsync([FromQuery] CheckPwnedPasswordRequest passwordRequest)
        {
            if (string.IsNullOrEmpty(passwordRequest.PASSWORD))
                return BadRequest();

            bool result = await _pwnedPasswords.HasPasswordBeenPwned(passwordRequest.PASSWORD.Trim());
            return Ok(result);
        }

        /// <summary>
        /// Endpoint action for accepting client's request to get all profiles available in system. 
        /// </summary>
        /// <returns>List of available profile details.</returns>
        /// <response code="200">List of profiles found successfully</response>
        [HttpGet(AccountMappings.GET_PROFILES)]
        [ProducesResponseType(typeof(List<ProfileCardResponse>), StatusCodes.Status200OK)]
        public IActionResult GetProfiles()
        {
            List<ProfileCardResponse> profileCardResponse = _accountManager.GetProfiles();
            return Ok(profileCardResponse);
        }

        /// <summary>
        /// Endpoint action for accepting client's request to change account's password
        /// </summary>
        /// <param name="changePasswordRequest">Request model for containing user's OLD_PASSOWRD, NEW_PASSWORD and CONFIRM_PASSWORD</param>
        /// <returns>Retruns HTTP Status 200 for successfull password change and 400 along with errors for unsuccessfull.</returns>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">Password change unsuccessful, check errors.</response> 
        [HttpPut(AccountMappings.CHANGE_PASSWORD)]
        [ProducesResponseType(typeof(ChangePasswordRequest), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            // get profile ID from user claims
            ClaimsIdentity userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string profileID = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ChangeRequestResponse response = _accountManager.ChangePassword(profileID, changePasswordRequest);

            if (response.IS_CHANGED)
                return Ok(response);
            else
                return BadRequest(response.ERRORS);
        }

        /// <summary>
        /// Endpoint action for accepting 
        /// </summary>
        /// <returns></returns>        
        [HttpPost(AccountMappings.SEND_VERIFICATION_CODE)]
        [ProducesResponseType(typeof(VerificationCodeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendVerificationCodeAsync()
        {
            ClaimsIdentity userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string profileID = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var token = HttpContext.Request.Headers[HeaderNames.Authorization];

            var header = new Metadata
            {
                { "Authorization", $"{token}" }
            };


            Profile profile = _accountManager.GetProfile(profileID);

            VerificationCodeResponse response = new();

            string emailBody = await _customRazorEngine.RazorViewToHtmlAsync<OtpView>("EmailTemplate/OtpEmail.cshtml", new()
            {
                OTP = _accountManager.GenerateOTP(profileID)
            });

            List<string> sendTO = new();
            sendTO.Add(profile.EMAIL);
            var result = await _notificationClient.SendEmailAsync(new()
            {
                TO = sendTO,
                SUBJECT = "Test",
                CONTENT = emailBody,
                HTML = true

            }, header);

            response.SENT = (bool)result.SENT;

            return Ok(response);
        }

        /// <summary>
        /// Endpoint action for verifying profile and marking profile as verified if otp matches.
        /// </summary>
        /// <param name="verifyProfileRequest"></param>
        /// <returns></returns>
        [HttpPost(AccountMappings.VERIFY_PROFILE)]
        [ProducesResponseType(typeof(VerifyProfileResponse), StatusCodes.Status200OK)]
        public IActionResult VerifyProfile(VerifyProfileRequest verifyProfileRequest)
        {
            ClaimsIdentity userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string profileID = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            VerifyProfileResponse response = new();
            response.VERIFIED = _accountManager.VerifyProfile(profileID, verifyProfileRequest.OTP.Trim());

            return Ok(response);
        }

        /// <summary>
        /// Endpoint action for deleting profile.
        /// </summary>
        /// <param name="deleteProfile"></param>
        /// <returns></returns>
        [HttpPatch(AccountMappings.DELETE_PROFILE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteProfile(ProfileRequest deleteProfile)
        {
            bool isDeleted = _accountManager.DeleteProfile(deleteProfile.PROFILE_ID);

            if (!isDeleted)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileRequest"></param>
        /// <returns></returns>
        [HttpGet(AccountMappings.GET_PROFILE_VIEW)]
        public IActionResult GetProfileView(ProfileRequest profileRequest)
        {
            ClaimsIdentity userIdentity = HttpContext.User.Identity as ClaimsIdentity;
            string profileID = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            if (!string.IsNullOrEmpty(profileID))
            {
                var profile = _accountManager.GetProfile(profileID);
                return Ok(profile);
            }
            else
                return BadRequest();

        }
    }
}
