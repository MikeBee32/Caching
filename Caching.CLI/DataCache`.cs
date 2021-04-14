using System;
using System.Collections.Specialized;
using System.Runtime.Caching;

namespace Caching.CLI
{
    public class DataCache<T>
    {
        private readonly object _lock = new object();
        private readonly MemoryCache _cache;
        private readonly TimeSpan _expiryTime;

        public DataCache(
            int cacheSize = 100,
            int pollInterval = 5,
            int expiryTime = 20)
        {
            var cacheSettings = new NameValueCollection
            {
                { "CacheMemoryLimitMegabytes", $"{cacheSize}" },
                { "PollingInterval", $"{TimeSpan.FromMinutes(pollInterval):hh\\:mm\\:ss}" }
            };
            _cache = new MemoryCache(typeof(T).Name, cacheSettings);
            _expiryTime = TimeSpan.FromMinutes(expiryTime);
        }

        public T GetOrAdd(int keyValue, Func<int, T> create)
        {
            lock (_lock)
            {
                var key = $"{keyValue}";
                var cacheItem = _cache.GetCacheItem(key);

                T value;

                if (cacheItem == null)
                {
                    value = create(keyValue);
                    cacheItem = new CacheItem(key, value);
                    var expirationPolicy = new CacheItemPolicy { SlidingExpiration = _expiryTime };
                    _cache.Add(cacheItem, expirationPolicy);
                }
                else
                {
                    value = (T)cacheItem.Value;
                }

                return value;
            }
        }
    }
}
