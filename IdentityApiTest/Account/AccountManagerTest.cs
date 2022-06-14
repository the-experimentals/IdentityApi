using System;
using IdentityApi.Account;
using IdentityApiTest.Data;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Xunit;

namespace IdentityApiTest.Account
{
    [TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
    public class AccountManagerTest : IClassFixture<IdentityStoreMock>
    {
        private readonly IAccountManager _accountManager;
        private static string testProfileGUID = Guid.NewGuid().ToString();
        public AccountManagerTest(IdentityStoreMock mock)
        {
            _accountManager = new AccountManager(mock._store , null);
        }

        [Fact(DisplayName = "Test create new profile"), Priority(1)]
        void TestCreateNewProfile()
        {
           var result =  _accountManager.CreateProfile(new()
           {
                ID = testProfileGUID,
                NAME = "test profile",
                NEW = true,
                CREATED_BY = "system",
                EMAIL = "test@test.com",
                CREDENTIAL = new()
                {
                    USERNAME = "testProfile",
                    PASSWORD = "TESTPASSWORD"
                },
                PERSON = new()
                {
                    FIRST_NAME = "test",
                    LAST_NAME = "profile",
                    PROFILE_ID = testProfileGUID,
                }
           });

            Assert.True(result.IS_SAVED);
            Assert.Equal(result.PROFILE_ID, testProfileGUID);
        }

        [Fact(DisplayName = "Test delete invalid profile"), Priority(2)]
        void TestDeleteInvalidProfile() => Assert.Throws<InvalidOperationException>(() => _accountManager.DeleteProfile(new Guid().ToString()));

        [Fact(DisplayName = "Test delete profile"), Priority(3)]
        void TestDeleteProfile() => Assert.True(_accountManager.DeleteProfile(DummyAccountData.profile.ID));

    }
}
