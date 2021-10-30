using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IdentityApi.Auth;
using IdentityApi.Data;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace IdentityApiTest.Auth
{
    [TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
    public class AuthManagerTest : IClassFixture<IdentityStoreMock>, IClassFixture<TMCacheMock>
    {
        private readonly IAuthManager _authManager;

        JwtSecretKey jwtSecretKey = null;

        public AuthManagerTest(IdentityStoreMock storeMock, TMCacheMock cacheMock)
        {
            jwtSecretKey = new()
            {
                SECRET = "Rh?jkJ4847wBXqvWCB5UbNZDnc&KN7ABxk^dpu43H9@f_Gt@FT@D=yHj?R!^ZTuEHN8Vb36-gNua5aak24fX=&g-+AUmS%?Udm3H6WT7h^W@AMhX!TzfbTCw?Z_XsBj6",
                ISSUER = "TMSolution",
                AUDIENCE = "TMSolution",
                TTL = 5
            };

            _authManager = new AuthManager(storeMock._store, cacheMock._cache, Options.Create<JwtSecretKey>(jwtSecretKey));
        }

        [Theory(DisplayName ="Test authenticate user."), Priority(1)]
        [ClassData(typeof(LogInRequestTestData))]
        void TestAuthenticate(LogInRequest logInRequest)
        {
            LogInResponse result = _authManager.Authenticate(logInRequest);

            Assert.True(result.IS_AUTHENTICATED);
        }

        [Fact(DisplayName ="Test username not found"), Priority(2)]
        void TestUserNameNotFound()
        {
            LogInRequest request = new()
            {
                USERNAME = "invalidUser",
                PASSWORD = "invallidPassword"
            };

            LogInResponse response =  _authManager.Authenticate(request);

            Assert.Equal("User not found", response.ERRORS[0]);
        }

        [Fact(DisplayName ="Test generating jwt token"), Priority(3)]
        void TestGenerateJWTToken()
        {
            LogInRequest request = new()
            {
                USERNAME = "default",
                PASSWORD = "defaultTest"
            };

            LogInResponse result = _authManager.Authenticate(request);

            Assert.True(result.IS_AUTHENTICATED);

            string token = _authManager.GenerateJwtToken(result);

            Assert.NotNull(token);

            var profileID = VerifyJWTToken(token);

            Assert.Equal(result.PROFILE_ID, profileID);
        }

        // Test Refresh token CRUD and in cache

        private string VerifyJWTToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Invalid token.");
            }

            
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(jwtSecretKey.SECRET);

            ClaimsPrincipal claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero

            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var profileID = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid").Value;

            return profileID;
        }
    }
}
