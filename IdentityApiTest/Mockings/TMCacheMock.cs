using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Utilities;
using IdentityApiTest.Data;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityApiTest.Mockings
{
    public class TMCacheMock : IDisposable
    {
        public TMCache _cache { get; private set; }
        

        public TMCacheMock()
        {
            _cache = new(new MemoryCache(new MemoryCacheOptions()));

            Profile profile = DummyAccountData.cacheProfile;

            _cache.Add<string>("testcacheuser", profile.ID);

            _cache.Add<Profile>(Profile.PROFILE_CACHE_KEY + profile.ID, profile);
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
