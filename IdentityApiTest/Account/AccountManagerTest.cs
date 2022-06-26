using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApiTest.Data;
using IdentityApiTest.Mockings;
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

    [Fact(DisplayName = "Test create new profile with username already exist")]
    private void TestUsernameAlreadyExist()
    {
        var result = _accountManager.CreateProfile(new Profile
        {
            ID = testProfileGUID,
            NAME = "test profile",
            NEW = true,
            CREATED_BY = "system",
            EMAIL = "test@test.com",
            CREDENTIAL = new Credential { USERNAME = "default", PASSWORD = "TESTPASSWORD" },
            PERSON = new Person { FIRST_NAME = "test", LAST_NAME = "profile", PROFILE_ID = testProfileGUID }
        });

        Assert.False(result.IS_SAVED);
        Assert.Equal("Username already exist. Please use other username to register.", result.ERRORS[0]);
    }

    [Fact(DisplayName = "Test creating profile with existing profile ID")]
    private void TestCreatingProfileWithExistingID()
    {
        Assert.Throws<InvalidOperationException>(() => _accountManager.CreateProfile(new Profile
        {
            ID = DummyData.defaultProfile.ID,
            NAME = "test profile",
            NEW = true,
            CREATED_BY = "system",
            EMAIL = "test@test.com",
            CREDENTIAL = new Credential { USERNAME = "testProfile", PASSWORD = "TESTPASSWORD" },
            PERSON = new Person { FIRST_NAME = "test", LAST_NAME = "profile", PROFILE_ID = testProfileGUID }
        }));

    }

    [Fact(DisplayName = "Test delete invalid profile")]
    private void TestDeleteInvalidProfile()
    {
        Assert.Throws<InvalidOperationException>(() => _accountManager.DeleteProfile(new Guid().ToString()));
    }

    [Fact(DisplayName = "Test delete profile")]
    private void TestDeleteProfile()
    {
        Assert.True(_accountManager.DeleteProfile(DummyData.defaultProfile.ID));
    }
}
