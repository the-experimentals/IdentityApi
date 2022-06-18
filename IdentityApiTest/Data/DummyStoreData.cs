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

        static string storeProfileGuid = Guid.NewGuid().ToString();

        static UserSecret userSecret = Utility.GetUserSecret(null, "defaultTest");

        public static Credential credential = new()
        {
            ID = Guid.NewGuid().ToString(),
            USERNAME = "default",
            PROFILE_ID = storeProfileGuid,
            SALT = userSecret.SALT,
            SECRET_HASH = userSecret.SECRET_HASH
        };

        public static Profile profile = new()
        {
            ID = storeProfileGuid,
            NAME = "default test user",
            NEW = true,
            CREATED_BY = "system",
            EMAIL = "test@test.com",
            CREDENTIAL = credential
        };

        public static RefreshToken refreshToken = new()
        {
            ID = Guid.NewGuid().ToString(),
            PROFILE_ID = storeProfileGuid,
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
            SHA = Utility.ComputeSHA(string.Concat(storeProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))

        };

        // Refresh token with expired token time.

        public static RefreshToken expiredRefreshToken = new()
        {
            ID = Guid.NewGuid().ToString(),
            PROFILE_ID = storeProfileGuid,
            BROWSER = "",
            DEVICE = "",
            OS = "",
            GENERATED_ON = DateTime.UtcNow.AddDays(-2),
            LIFE_SPAN = 1,
            STATUS = IdentityApi.Identifiers.Status.ACTIVE,
            ACTIVE = true,
            IPv4 = "127.0.0.1",
            TOKEN = Utility.GetUniqueString(32),
            REFRESHED_ON = DateTime.UtcNow,
            SHA = Utility.ComputeSHA(string.Concat(storeProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))

        };

    }
}
