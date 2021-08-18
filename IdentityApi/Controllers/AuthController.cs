using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using IdentityApi.Auth;
using IdentityApi.Mappings;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route(AuthMappings.ENDPOINT_ROUTE)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthController : Controller
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Testing auth endpoint");
        }

        /// <summary>
        /// Endpoint action for accepting client's request to validate account credentials.
        /// </summary>
        /// <param name="logInRequest">Request model for containing user's IDENTIFIER and SECRET.</param>
        /// <returns>Login response which either contains authentication success status or errors for faliure.</returns>
        /// <response code="200">User's credentials have been authenticated successfully</response>
        /// <response code="401">Unsuccessfull user's authentication, check errors in response model.</response> 
        [AllowAnonymous]
        [HttpPost(AuthMappings.LOG_IN)]
        [ProducesResponseType(typeof(LogInResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status401Unauthorized)]
        public IActionResult LogIn([FromBody]LogInRequest logInRequest)
        {
            LogInResponse logInResponse;


            logInResponse = _authManager.Authenticate(logInRequest);

            if (logInResponse.IS_AUTHENTICATED)
            {
                string token = _authManager.GenerateJwtToken(logInResponse);

                if (logInResponse.IS_VERIFIED)
                {

                }

                logInResponse.TOKEN.ACCESS = token;
            }            

            return Ok(logInResponse);            
        }
    }
}
