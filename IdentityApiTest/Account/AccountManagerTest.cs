using System;
using IdentityApi.Account;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Xunit;

namespace IdentityApiTest.Account
{
    [TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
    public class AccountManagerTest : IClassFixture<IdentityStoreMock>
    {
        private readonly IAccountManager _accountManager;
        public AccountManagerTest(IdentityStoreMock mock)
        {
            _accountManager = new AccountManager(mock._store);
        }

        [Fact(DisplayName = "Test create new profile"), Priority(1)]
        public void TestCreateNewProfile()
        {
           string profileGUID = Guid.NewGuid().ToString();
           var result =  _accountManager.CreateProfile(new()
           {
                ID = profileGUID,
                NAME = "test profile",
                NEW = true,
                CREATED_BY = "system",
                EMAIL = "test@test.com",
                CREDENTIAL = new()
                {
                    USERNAME = "testProfile",
                    PASSWORD = "TESTPASSWORD"
                }
           });

            Assert.True(result.IS_SAVED);
            Assert.Equal(result.PROFILE_ID, profileGUID);
        }
    }
}
