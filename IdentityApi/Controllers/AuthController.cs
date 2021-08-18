using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Grpc.Core;
using IdentityApi.Auth;
using IdentityApi.Mappings;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApi.Services.gRPC.Clients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UAParser;

namespace IdentityApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route(AuthMappings.ENDPOINT_ROUTE)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AuthController : Controller
    {
        private readonly IAuthManager _authManager;
        private readonly PolicyApiClient _policyApiClient;

        public AuthController(IAuthManager authManager, PolicyApiClient policyApiClient)
        {
            _authManager = authManager;
            _policyApiClient = policyApiClient;
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
        public async Task<IActionResult> LogInAsync([FromBody]LogInRequest logInRequest)
        {
            LogInResponse logInResponse;


            logInResponse = _authManager.Authenticate(logInRequest);

            if (logInResponse.IS_AUTHENTICATED)
            {
                string token = _authManager.GenerateJwtToken(logInResponse);

                if (logInResponse.IS_VERIFIED)
                {
                    var header = new Metadata
                    {
                        { "Authorization", $"Bearer {token}" }
                    };

                    var policyResponse = await _policyApiClient.GetPolicyTokenAsync(header);

                    if (policyResponse == null)
                        return StatusCode(StatusCodes.Status500InternalServerError);

                    token = policyResponse.ACCESS;

                    logInResponse.TOKEN.ALLOW_REFRESH = true;
                    logInResponse.TOKEN.REFRESH = _authManager.GetOrCreateRefreshToken(logInResponse.PROFILE_ID, GetClientInfo(), GetIPAddress()).TOKEN;
                }

                logInResponse.TOKEN.ACCESS = token;
            }            

            return Ok(logInResponse);            
        }

        private RequestModels.UserAgent GetClientInfo()
        {
            RequestModels.UserAgent ua = new();


            var userAgent = HttpContext.Request.Headers["User-Agent"];
            string uaString = Convert.ToString(userAgent.FirstOrDefault());
            Parser uaParser = Parser.GetDefault();

            ClientInfo clientInfo = uaParser.Parse(uaString);
            ua.DEVICE = clientInfo.Device.Family;
            ua.OS = clientInfo.OS.Family;
            ua.BROWSER = clientInfo.UA.Family;
            return ua;
        }

        private IPAddress GetIPAddress()
        {
            IPAddress ipAddress = Request.HttpContext.Connection.RemoteIpAddress;

            if (ipAddress != null)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    ipAddress = System.Net.Dns.GetHostEntry(ipAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                }
            }

            return ipAddress;
        }
    }
}
