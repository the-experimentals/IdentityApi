using System;
using System.Net;
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

        public static RefreshToken refreshToken = new()
        {
            ID = Guid.NewGuid().ToString(),
            PROFILE_ID = testProfileGuid,
            BROWSER = "",
            DEVICE = "",
            OS = "",
            GENERATED_ON = DateTime.UtcNow,
            LIFE_SPAN = 1,
            STATUS = IdentityApi.Identifiers.Status.ACTIVE,
            ACTIVE = true,
            IPv4 = "127.0.0.1",
            TOKEN = Utility.GetUniqueString(32),
            REFRESHED_ON = DateTime.UtcNow,
            SHA = Utility.ComputeSHA(string.Concat(testProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))

        };
    }
}
