using System;
using IdentityApi.DataModels;
using IdentityApi.Utilities;
using IdentityApiTest.Data;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityApiTest.Mockings;

public class TMCacheMock : IDisposable
{
    public TMCacheMock()
    {
        _cache = new TMCache(new MemoryCache(new MemoryCacheOptions()));

        var profile = DummyData.cacheProfile;

        _cache.Add("testuser", profile.ID);

        _cache.Add(Profile.PROFILE_CACHE_KEY + profile.ID, profile);

        _cache.Add(RefreshToken.REFRESH_TOKEN_CACHE_KEY + DummyData.cacheProfileRefreshToken.SHA,
            DummyData.cacheProfileRefreshToken);
    }

    public TMCache _cache { get; }

    public void Dispose()
    {
        _cache.Dispose();
    }
}
