using System.Collections.Concurrent;

namespace WebscrapingBenchmark.Core.Framework.Caching
{
    public class Cache<TType> : ICache<TType> where TType : class
    {
        private readonly ConcurrentDictionary<string, TType> _cache = new();

        public TType? TryGet(string key)
        {
            return _cache.TryGetValue(key, out var result) ? result : null;
        }

        public void AddOrUpdate(string key, TType value)
        {
            _cache.AddOrUpdate(key, value, (_, _) => value);
        }

        public TType GetOrAdd(string key, Func<string, TType> function)
        {
            return _cache.GetOrAdd(key, function);
        }

        public bool Contains(string key)
        {
            return _cache.ContainsKey(key);
        }
    }

    public class CachedRequest
    {
        public string htmlBody;
        public TimeSpan Delay;
    }
}
