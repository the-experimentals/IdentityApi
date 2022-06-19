using System;
using System.Net;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Identifiers;
using IdentityApi.Utilities;

namespace IdentityApiTest.Data;

public class DummyData
{
    // Data set 1

    private static readonly string defaultProfileGuid = Guid.NewGuid().ToString();

    private static readonly UserSecret defaultProfileuserSecret = Utility.GetUserSecret(null, "defaultTest");

    public static Credential defaultProfilecredential = new()
    {
        ID = Guid.NewGuid().ToString(),
        USERNAME = "default",
        PROFILE_ID = defaultProfileGuid,
        SALT = defaultProfileuserSecret.SALT,
        SECRET_HASH = defaultProfileuserSecret.SECRET_HASH
    };

    public static Profile defaultProfile = new()
    {
        ID = defaultProfileGuid,
        NAME = "default test user",
        NEW = true,
        CREATED_BY = "system",
        EMAIL = "test@test.com",
        CREDENTIAL = defaultProfilecredential
    };

    public static RefreshToken defaultProfileRefreshToken = new()
    {
        ID = Guid.NewGuid().ToString(),
        PROFILE_ID = defaultProfileGuid,
        BROWSER = "",
        DEVICE = "",
        OS = "",
        GENERATED_ON = DateTime.UtcNow,
        LIFE_SPAN = 1,
        STATUS = Status.ACTIVE,
        ACTIVE = true,
        IPv4 = "127.0.0.1",
        TOKEN = Utility.GetUniqueString(32),
        REFRESHED_ON = DateTime.UtcNow,
        SHA = Utility.ComputeSHA(string.Concat(defaultProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))
    };

    // Data set 2

    private static readonly string cacheProfileGuid = Guid.NewGuid().ToString();

    private static readonly UserSecret cacheProfileuserSecret = Utility.GetUserSecret(null, "defaultTest");

    public static Credential cacheProfilecredential = new()
    {
        ID = Guid.NewGuid().ToString(),
        USERNAME = "default",
        PROFILE_ID = cacheProfileGuid,
        SALT = cacheProfileuserSecret.SALT,
        SECRET_HASH = cacheProfileuserSecret.SECRET_HASH
    };

    public static Profile cacheProfile = new()
    {
        ID = cacheProfileGuid,
        NAME = "cache test user",
        NEW = true,
        CREATED_BY = "system",
        EMAIL = "test@test.com",
        CREDENTIAL = cacheProfilecredential
    };

    public static RefreshToken cacheProfileRefreshToken = new()
    {
        ID = Guid.NewGuid().ToString(),
        PROFILE_ID = cacheProfileGuid,
        BROWSER = "",
        DEVICE = "",
        OS = "",
        GENERATED_ON = DateTime.UtcNow,
        LIFE_SPAN = 1,
        STATUS = Status.ACTIVE,
        ACTIVE = true,
        IPv4 = "127.0.0.1",
        TOKEN = Utility.GetUniqueString(32),
        REFRESHED_ON = DateTime.UtcNow,
        SHA = Utility.ComputeSHA(string.Concat(cacheProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))
    };

    // Data set 3

    private static readonly string deleteProfileGuid = Guid.NewGuid().ToString();

    private static readonly UserSecret deleteProfileuserSecret = Utility.GetUserSecret(null, "defaultTest");

    public static Credential deleteProfilecredential = new()
    {
        ID = Guid.NewGuid().ToString(),
        USERNAME = "default",
        PROFILE_ID = deleteProfileGuid,
        SALT = deleteProfileuserSecret.SALT,
        SECRET_HASH = deleteProfileuserSecret.SECRET_HASH
    };

    public static Profile deleteProfile = new()
    {
        ID = deleteProfileGuid,
        NAME = "delete test user",
        NEW = true,
        CREATED_BY = "system",
        EMAIL = "test@test.com",
        CREDENTIAL = cacheProfilecredential
    };

    public static RefreshToken deleteProfileRefreshToken = new()
    {
        ID = Guid.NewGuid().ToString(),
        PROFILE_ID = deleteProfileGuid,
        BROWSER = "",
        DEVICE = "",
        OS = "",
        GENERATED_ON = DateTime.UtcNow,
        LIFE_SPAN = 1,
        STATUS = Status.ACTIVE,
        ACTIVE = true,
        IPv4 = "127.0.0.1",
        TOKEN = Utility.GetUniqueString(32),
        REFRESHED_ON = DateTime.UtcNow,
        SHA = Utility.ComputeSHA(string.Concat(deleteProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))
    };

    // Data set 4

    private static readonly string ericProfileGuid = Guid.NewGuid().ToString();

    private static readonly UserSecret ericProfileuserSecret = Utility.GetUserSecret(null, "defaultTest");

    public static Credential ericProfilecredential = new()
    {
        ID = Guid.NewGuid().ToString(),
        USERNAME = "default",
        PROFILE_ID = ericProfileGuid,
        SALT = ericProfileuserSecret.SALT,
        SECRET_HASH = ericProfileuserSecret.SECRET_HASH
    };

    public static Profile ericProfile = new()
    {
        ID = ericProfileGuid,
        NAME = "delete test user",
        NEW = true,
        CREATED_BY = "system",
        EMAIL = "test@test.com",
        CREDENTIAL = cacheProfilecredential
    };

    public static RefreshToken ericProfileRefreshToken = new()
    {
        ID = Guid.NewGuid().ToString(),
        PROFILE_ID = ericProfileGuid,
        BROWSER = "",
        DEVICE = "",
        OS = "",
        GENERATED_ON = DateTime.UtcNow.AddDays(-2),
        LIFE_SPAN = 1,
        STATUS = Status.ACTIVE,
        ACTIVE = true,
        IPv4 = "127.0.0.1",
        TOKEN = Utility.GetUniqueString(32),
        REFRESHED_ON = DateTime.UtcNow,
        SHA = Utility.ComputeSHA(string.Concat(ericProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))
    };

    private static readonly string amyProfileGuid = Guid.NewGuid().ToString();

    private static UserSecret amyProfileuserSecret = Utility.GetUserSecret(null, "defaultTest");

    public static Credential amyProfilecredential = new()
    {
        ID = Guid.NewGuid().ToString(),
        USERNAME = "default",
        PROFILE_ID = ericProfileGuid,
        SALT = ericProfileuserSecret.SALT,
        SECRET_HASH = ericProfileuserSecret.SECRET_HASH
    };

    public static Profile amyProfile = new()
    {
        ID = amyProfileGuid,
        NAME = "delete test user",
        NEW = true,
        CREATED_BY = "system",
        EMAIL = "test@test.com",
        CREDENTIAL = cacheProfilecredential
    };

    public static RefreshToken amyProfileRefreshToken = new()
    {
        ID = Guid.NewGuid().ToString(),
        PROFILE_ID = amyProfileGuid,
        BROWSER = "",
        DEVICE = "",
        OS = "",
        GENERATED_ON = DateTime.UtcNow.AddDays(-2),
        LIFE_SPAN = 1,
        STATUS = Status.DELETED,
        ACTIVE = true,
        IPv4 = "127.0.0.1",
        TOKEN = Utility.GetUniqueString(32),
        REFRESHED_ON = DateTime.UtcNow,
        SHA = Utility.ComputeSHA(string.Concat(amyProfileGuid, "", "", "", IPAddress.Parse("127.0.0.1")))
    };
}
