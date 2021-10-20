using System;
using IdentityApi.Auth;
using IdentityApi.Data;
using IdentityApi.RequestModels;
using IdentityApi.ResponseModels;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace IdentityApiTest.Auth
{
    [TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
    public class AuthManagerTest : IClassFixture<IdentityStoreMock>, IClassFixture<TMCacheMock>
    {
        private readonly IAuthManager _authManager;
        public AuthManagerTest(IdentityStoreMock storeMock, TMCacheMock cacheMock)
        {            
            _authManager = new AuthManager(storeMock._store, cacheMock._cache, Options.Create<JwtSecretKey>(new JwtSecretKey()));
        }

        [Theory(DisplayName ="Test authenticate user."), Priority(1)]
        [ClassData(typeof(LogInRequestTestData))]
        void TestAuthenticate(LogInRequest logInRequest)
        {
            LogInResponse result = _authManager.Authenticate(logInRequest);

            Assert.True(result.IS_AUTHENTICATED);
        }

        [Fact(DisplayName ="Test username not found")]
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
    }
}
