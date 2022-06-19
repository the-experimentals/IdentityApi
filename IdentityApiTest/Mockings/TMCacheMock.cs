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

            Profile profile = DummyData.cacheProfile;

            _cache.Add<string>("testuser", profile.ID);

            _cache.Add<Profile>(Profile.PROFILE_CACHE_KEY + profile.ID, profile);

            _cache.Add<RefreshToken>(RefreshToken.REFRESH_TOKEN_CACHE_KEY + DummyData.cacheProfileRefreshToken.SHA, DummyData.cacheProfileRefreshToken);

        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
