using System;
using System.Runtime.Caching;

namespace NewsieBot.Utilities
{
    /// <summary>
    /// This is a simple cache manager
    /// ***IMPORTANT NOTE***: This is just a simple cache manager for sample purposes 
    /// not an efficient way to handle caching. If this code got deployed on multiple
    /// instances there would be a lot of cache misses, which might affect the scenario
    /// </summary>
    internal sealed class CacheManager : ICacheManager
    {
        private readonly ObjectCache cache;

        public CacheManager()
        {
            this.cache = MemoryCache.Default;
        }

        public void Write<T>(string key, T value)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            this.cache.Add(key, value, policy);
        }

        public bool TryRead<T>(string urlKey, out T value)
        {
            value = (T)this.cache.Get(urlKey);
            return this.cache.Contains(urlKey);
        }
    }
}