using System;
using Microsoft.Extensions.Caching.Memory;

namespace IdentityApi.Utilities
{
    public class TMCache
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Constructor to initialize IMemoryCache from dependency injection
        /// </summary>
        /// <param name="memoryCache">IMemoryCache</param>
        public TMCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Add T object to IMemoryCache
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="cacheKey">Unique key to store object</param>
        /// <param name="data">Object of type T</param>
        public void Add<T>(string cacheKey, T data)
        {
            // Look for cache key.
            if (!_cache.TryGetValue(cacheKey, out T cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = data;

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetAbsoluteExpiration(TimeSpan.FromDays(7));

                // Save data in cache.
                _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }
        }

        /// <summary>
        /// Return object of type T from IMemoryCache
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="cacheKey">Unique key for object stored in cache</param>
        /// <returns></returns>
        public T Get<T>(string cacheKey)
        {
            return _cache.Get<T>(cacheKey);
        }

        /// <summary>
        /// Removes object from IMemoryCache
        /// </summary>
        /// <param name="key">Unique key for object stored in cache</param>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Remove old object and add new object for same key present in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Refresh<T>(string key, T data)
        {
            Remove(key);
            Add<T>(key, data);            
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}
