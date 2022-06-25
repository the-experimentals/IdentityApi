using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using IdentityApi.Auth;
using IdentityApi.Data;
using IdentityApi.Identifiers;
using IdentityApi.RequestModels;
using IdentityApiTest.Data;
using IdentityApiTest.Mockings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityApiTest.Auth;

[TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
public class AuthManagerTest : IClassFixture<IdentityStoreMock>, IClassFixture<TMCacheMock>
{
    private readonly IAuthManager _authManager;

    private readonly JwtSecretKey jwtSecretKey;

    public AuthManagerTest(IdentityStoreMock storeMock, TMCacheMock cacheMock)
    {
        jwtSecretKey = new JwtSecretKey
        {
            SECRET =
                "Rh?jkJ4847wBXqvWCB5UbNZDnc&KN7ABxk^dpu43H9@f_Gt@FT@D=yHj?R!^ZTuEHN8Vb36-gNua5aak24fX=&g-+AUmS%?Udm3H6WT7h^W@AMhX!TzfbTCw?Z_XsBj6",
            ISSUER = "TMSolution",
            AUDIENCE = "TMSolution",
            TTL = 5
        };

        _authManager = new AuthManager(storeMock._store, cacheMock._cache, Options.Create(jwtSecretKey));
    }

    [Theory(DisplayName = "Test authenticate user.")]
    [ClassData(typeof(LogInRequestTestData))]
    private void TestAuthenticate(LogInRequest logInRequest)
    {
        var result = _authManager.Authenticate(logInRequest);

        Assert.True(result.IS_AUTHENTICATED);
    }

    [Fact(DisplayName = "Test username not found")]
    private void TestUserNameNotFound()
    {
        LogInRequest request = new() { USERNAME = "invalidUser", PASSWORD = "invallidPassword" };

        var response = _authManager.Authenticate(request);

        Assert.Equal("User not found", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test login with locked account")]
    private void TestLoginWithlockedaccroun()
    {
        LogInRequest logInRequest = new()
        {
            USERNAME = "eric",
            PASSWORD = "ericPassword"
        };

        var response = _authManager.Authenticate(logInRequest);

        Assert.False(response.IS_AUTHENTICATED);

        Assert.Equal("Profile locked due to many invalid attempts to login. Contact administartor for assistance", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test login with deactivated account")]
    private void TestLoginWithDeactivatedAccount()
    {
        LogInRequest logInRequest = new()
        {
            USERNAME = "amy",
            PASSWORD = "amyPassword"
        };

        var response = _authManager.Authenticate(logInRequest);

        Assert.False(response.IS_AUTHENTICATED);

        Assert.Equal("Your profile is temporarily deactivated. To activate your profile conatct administrator.", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test login with invlid password")]
    private void TestLoginWithInvalidPassword()
    {
        LogInRequest request = new() { USERNAME = "amy", PASSWORD = "incorrectPassword" };

        var response = _authManager.Authenticate(request);

        Assert.False(response.IS_AUTHENTICATED);

        Assert.Equal("Invalid password", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test system user with invalid password")]
    private void TestSystemUserWithInvalidPassword()
    {
        LogInRequest logInRequest = new()
        {
            USERNAME = "system",
            PASSWORD = "invalidPassword"
        };

        var response = _authManager.Authenticate(logInRequest);

        Assert.False(response.IS_AUTHENTICATED);

        Assert.Equal("Invalid password", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test login to locked user.")]
    private void TestLoginLockedUser()
    {
        LogInRequest logInRequest = new()
        {
            USERNAME = "eric",
            PASSWORD = "ericPassword"
        };

        var response = _authManager.Authenticate(logInRequest);

        Assert.False(response.IS_AUTHENTICATED);

        Assert.Equal("Profile locked due to many invalid attempts to login. Contact administartor for assistance", response.ERRORS[0]);
    }

    [Fact(DisplayName = "Test generating jwt token")]
    private void TestGenerateJWTToken()
    {
        LogInRequest request = new() { USERNAME = "default", PASSWORD = "defaultTest" };

        var result = _authManager.Authenticate(request);

        Assert.True(result.IS_AUTHENTICATED);

        var token = _authManager.GenerateJwtToken(result);

        Assert.NotNull(token);

        var profileID = VerifyJWTToken(token);

        Assert.Equal(result.PROFILE_ID, profileID);
    }

    // Test Refresh token CRUD and in cache

    [Fact(DisplayName = "Test generate refresh token")]
    private void TestGenerateRefreshToken()
    {
        var profileID = Guid.NewGuid().ToString();

        UserAgent ua = new() { BROWSER = "", DEVICE = "", OS = "" };

        var ipAddress = IPAddress.Parse("127.0.0.1");

        Assert.NotNull(_authManager.GenerateRefreshToken(profileID, ua, ipAddress));
    }

    [Fact(DisplayName = "Test get refresh token")]
    private void TestGetRefreshToken()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.defaultProfileRefreshToken.BROWSER,
            DEVICE = DummyData.defaultProfileRefreshToken.DEVICE,
            OS = DummyData.defaultProfileRefreshToken.OS
        };

        var refreshToken = _authManager.GetRefreshToken(DummyData.defaultProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.NotNull(refreshToken);

        Assert.Equal(DummyData.defaultProfile.ID, refreshToken.PROFILE_ID);
    }

    [Fact(DisplayName = "Test get refresh token with null user agent")]
    private void TestGetRefreshTokenWithNullUserAgent()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _authManager.GetRefreshToken(DummyData.defaultProfile.ID, null, IPAddress.Parse("127.0.0.1")));
    }

    [Fact(DisplayName = "Test delete reefresh token")]
    private void TestDeleteRefreshToken()
    {
        var isDeleted = _authManager.DeleteRefreshToken(DummyData.deleteProfileRefreshToken);

        Assert.True(isDeleted);

        UserAgent ua = new()
        {
            BROWSER = DummyData.deleteProfileRefreshToken.BROWSER,
            DEVICE = DummyData.deleteProfileRefreshToken.DEVICE,
            OS = DummyData.deleteProfileRefreshToken.OS
        };

        var refreshToken = _authManager.GetRefreshToken(DummyData.deleteProfile.ID, ua, IPAddress.Parse("127.0.0.1"));
        Assert.Null(refreshToken);
    }

    [Fact(DisplayName = "Test get refresh token from cache")]
    private void TestGetRefreshTokenFromCache()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.cacheProfileRefreshToken.BROWSER,
            DEVICE = DummyData.cacheProfileRefreshToken.DEVICE,
            OS = DummyData.cacheProfileRefreshToken.OS
        };

        var refreshToken = _authManager.GetRefreshToken(DummyData.cacheProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.NotNull(refreshToken);

        Assert.Equal(DummyData.cacheProfile.ID, refreshToken.PROFILE_ID);
    }

    [Fact(DisplayName = "Test when refresh token has expired")]
    private void TestGetExpiredRefreshToken()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.ericProfileRefreshToken.BROWSER,
            DEVICE = DummyData.ericProfileRefreshToken.DEVICE,
            OS = DummyData.ericProfileRefreshToken.OS
        };

        var refreshToken = _authManager.GetRefreshToken(DummyData.ericProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.NotNull(refreshToken);

        Assert.NotEqual(DummyData.ericProfileRefreshToken.ID, refreshToken.ID);

        Assert.Equal(DummyData.ericProfile.ID, refreshToken.PROFILE_ID);
    }

    [Fact(DisplayName = "Test update refresh token")]
    private void TestUpdateRefreshToken()
    {
        var lastRefreshedON = DummyData.defaultProfileRefreshToken.REFRESHED_ON;
        var token = DummyData.defaultProfileRefreshToken.TOKEN;

        Assert.True(_authManager.UpdateRefreshToken(DummyData.defaultProfileRefreshToken));

        UserAgent ua = new()
        {
            BROWSER = DummyData.defaultProfileRefreshToken.BROWSER,
            DEVICE = DummyData.defaultProfileRefreshToken.DEVICE,
            OS = DummyData.defaultProfileRefreshToken.OS
        };

        var refreshToken = _authManager.GetRefreshToken(DummyData.defaultProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.NotEqual(refreshToken.TOKEN, token);

        Assert.Equal(lastRefreshedON.CompareTo(refreshToken.REFRESHED_ON), -1);
    }

    // get or generate refresh token.

    [Fact(DisplayName = "Test get or create refresh token")]
    private void TestGetOrCreateRefreshTokenGET()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.defaultProfileRefreshToken.BROWSER,
            DEVICE = DummyData.defaultProfileRefreshToken.DEVICE,
            OS = DummyData.defaultProfileRefreshToken.OS
        };

        var refreshToken =
            _authManager.GetOrCreateRefreshToken(DummyData.defaultProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.NotNull(refreshToken);

        Assert.Equal(DummyData.defaultProfileRefreshToken.TOKEN, refreshToken.TOKEN);
    }

    [Fact(DisplayName = "Test generate refresh token")]
    private void TestGetOrCreateRefreshTokenGENERATE()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.amyProfileRefreshToken.BROWSER,
            DEVICE = DummyData.amyProfileRefreshToken.DEVICE,
            OS = DummyData.amyProfileRefreshToken.OS
        };

        var refreshToken =
            _authManager.GetOrCreateRefreshToken(DummyData.amyProfile.ID, ua, IPAddress.Parse("127.0.0.1"));

        Assert.True(refreshToken.STATUS.Equals(Status.ACTIVE));

        Assert.NotEqual(refreshToken.TOKEN, DummyData.amyProfileRefreshToken.TOKEN);
    }

    [Fact(DisplayName = "Test logout")]
    private void TestLogout()
    {
        UserAgent ua = new()
        {
            BROWSER = DummyData.deleteProfileRefreshToken.BROWSER,
            DEVICE = DummyData.deleteProfileRefreshToken.DEVICE,
            OS = DummyData.deleteProfileRefreshToken.OS
        };

        Assert.True(_authManager.Logout(DummyData.defaultProfile.ID, ua, IPAddress.Parse("127.0.0.1")));

    }

    private string VerifyJWTToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Invalid token.");
        }


        JwtSecurityTokenHandler tokenHandler = new();
        var key = Encoding.ASCII.GetBytes(jwtSecretKey.SECRET);

        var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
            ClockSkew = TimeSpan.Zero
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var profileID = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid").Value;

        return profileID;
    }
}
