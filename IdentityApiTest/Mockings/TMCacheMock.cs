using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityApiTest.Mockings
{
    public class TMCacheMock : IDisposable
    {
        public TMCache _cache { get; private set; }
        public string testProfileGuid = Guid.NewGuid().ToString();

        public TMCacheMock()
        {
            _cache = new(new MemoryCache(new MemoryCacheOptions()));

            _cache.Add<string>("testuser", testProfileGuid);

            UserSecret userSecret = Utility.GetUserSecret(null, "testPassword");

            Credential credential = new()
            {
                ID = Guid.NewGuid().ToString(),
                USERNAME = "testuser",
                PROFILE_ID = testProfileGuid,
                SALT = userSecret.SALT,
                SECRET_HASH = userSecret.SECRET_HASH
            };

            Profile profile = new()
            {
                ID = testProfileGuid,
                NAME = "default test user",
                NEW = true,
                CREATED_BY = "system",
                EMAIL = "test@test.com",
                CREDENTIAL = credential
            };

            _cache.Add<Profile>(Profile.PROFILE_CACHE_KEY + profile.ID, profile);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
