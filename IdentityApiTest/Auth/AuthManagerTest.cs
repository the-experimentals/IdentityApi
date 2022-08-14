using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
            ISSUER = "TMSolution",
            AUDIENCE = "TMSolution",
            TTL = 5,
            PRIVATE_KEY = @"-----BEGIN RSA PRIVATE KEY-----
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
-----END RSA PRIVATE KEY-----
",
            PUBLIC_KEY = @"-----BEGIN PUBLIC KEY-----
MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAs9oweHsvOR6X0z2T4TC0
K7QY0wIlbnnG+v6QmsQlGqV3C3/0NQeURhkWkGTmI2O/HqjMKvCafWM1c8W6tU55
YVDZUSJ9/byazEryM7XzoKPU2mlTi0/NFOVBs0Ne8NmxyKz7giWPAl/c//yvaGw6
TPteNHqwtYxMVTWPe+YG6VfT+vyzVCbHz1FpWd/GGk7/9v2bXVbQnwN1AOb1rp/R
kQzMLpfPAXx1m+rwqxUnauCGzU6qkN2eBRM8bu/YpwtcXMQGng7c10/Bn47+2ne7
Jgl3TiAUlRp+thwXWkdoSSU4/5VPJMlC1Y80DlnLHq4+zOK8yAJB3FbmF4mAG8wn
TB3HNh0zRMOxro3+l9bPTovvRip/S8/OUV4Q+q9wGArAZJmlaYLCnx/GMXgLjRce
J6+XrmNzmyeXjm8tucjcJx8GZL7HcNMctuL5YBofEDuCDYKPOt0tVA95FZcwx1tw
yOWsUTtQ+e4z0jQyy1Kii9pptNT6OPY/5cLpg0mPW/DrbyEaEMAZlLKQEr3ygbEq
2WozkHvkj02bWvL9VklXS0cp/WLQorK1irE0tfOhmBMuadYZ2jiSTNWTGUQm41BU
Qm01wW+gW99LME3zFDXeFUOjVhFmUDDdnYrcg225U93NBdCvTOjjfI1qju8UZvUx
ftcnXiPc4Ye8CAJBduzVE80CAwEAAQ==
-----END PUBLIC KEY-----
"
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

        RSA rsa = RSA.Create();
        rsa.ImportFromPem(jwtSecretKey.PUBLIC_KEY.ToCharArray());

        var claims = tokenHandler.ValidateToken(token, new TokenValidationParameters()
        {
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidAudience = "TMSolution",
            ValidIssuer = "TMSolution",
            RequireSignedTokens = true,
            RequireExpirationTime = true, // <- JWTs are required to have "exp" property set
            ValidateLifetime = true, // <- the "exp" will be validated
            ValidateAudience = true,
            ValidateIssuer = true,
        }, out var validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var profileID = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid").Value;

        return profileID;

    }
}
