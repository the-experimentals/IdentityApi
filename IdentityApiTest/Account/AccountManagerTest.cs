using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApiTest.Data;
using IdentityApiTest.Mockings;
using IdentityApiTest.Ordering;
using Xunit;

namespace IdentityApiTest.Account;

[TestCaseOrderer("IdentityApiTest.Ordering.PriorityOrder", "IdentityApiTest")]
public class AccountManagerTest : IClassFixture<IdentityStoreMock>
{
    private static readonly string testProfileGUID = Guid.NewGuid().ToString();
    private readonly IAccountManager _accountManager;

    public AccountManagerTest(IdentityStoreMock mock)
    {
        _accountManager = new AccountManager(mock._store, null);
    }

    [Fact(DisplayName = "Test create new profile")]
    [Priority(1)]
    private void TestCreateNewProfile()
    {
        var result = _accountManager.CreateProfile(new Profile
        {
            ID = testProfileGUID,
            NAME = "test profile",
            NEW = true,
            CREATED_BY = "system",
            EMAIL = "test@test.com",
            CREDENTIAL = new Credential { USERNAME = "testProfile", PASSWORD = "TESTPASSWORD" },
            PERSON = new Person { FIRST_NAME = "test", LAST_NAME = "profile", PROFILE_ID = testProfileGUID }
        });

        Assert.True(result.IS_SAVED);
        Assert.Equal(result.PROFILE_ID, testProfileGUID);
    }

    [Fact(DisplayName = "Test delete invalid profile")]
    [Priority(2)]
    private void TestDeleteInvalidProfile()
    {
        Assert.Throws<InvalidOperationException>(() => _accountManager.DeleteProfile(new Guid().ToString()));
    }

    [Fact(DisplayName = "Test delete profile")]
    [Priority(3)]
    private void TestDeleteProfile()
    {
        Assert.True(_accountManager.DeleteProfile(DummyData.defaultProfile.ID));
    }
}
