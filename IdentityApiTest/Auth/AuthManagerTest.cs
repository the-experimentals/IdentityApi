using System;
using IdentityApi.Auth;
using IdentityApi.ResponseModels;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
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
            _authManager = new AuthManager(storeMock._store, cacheMock._cache);
        }

        [Fact(DisplayName = "Test authenictate user"), Priority(1)]
        public void TestAuthenticate()
        {
            LogInResponse result = _authManager.Authenticate(new()
            {
                USERNAME = "testuser",
                PASSWORD = "testPassword"
            });

            Assert.True(result.IS_AUTHENTICATED);
        }
    }
}
