using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Utilities;

namespace IdentityApiTest.Data
{
    public class DummyAccountData
    {
        // Data set 1

        static string testProfileGuid = Guid.NewGuid().ToString();

        static UserSecret userSecret = Utility.GetUserSecret(null, "defaultTest");

        public static Credential credential = new()
        {
            ID = Guid.NewGuid().ToString(),
            USERNAME = "default",
            PROFILE_ID = testProfileGuid,
            SALT = userSecret.SALT,
            SECRET_HASH = userSecret.SECRET_HASH
        };

        public static Profile profile = new()
        {
            ID = testProfileGuid,
            NAME = "default test user",
            NEW = true,
            CREATED_BY = "system",
            EMAIL = "test@test.com",
            CREDENTIAL = credential
        };
    }
}
