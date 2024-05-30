using System.Collections.Concurrent;
using Assignment.Application.Common.Interfaces;

namespace Assignment.UI.Cache;
public class SimpleCache<T> : ISimpleCache<T>
{
    private readonly object _lock = new object();
    private readonly TimeSpan _expirationTime;
    private readonly ConcurrentDictionary<string, CacheItem<T>> _cache;

    public SimpleCache(TimeSpan expirationTime)
    {
        _expirationTime = expirationTime;
        _cache = new ConcurrentDictionary<string, CacheItem<T>>();
    }

    public async Task<T> Get(string key)
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (cacheItem.ExpirationTime >= DateTime.Now)
            {
                return await Task.FromResult(cacheItem.Value);
            }
            else
            {
                _cache.TryRemove(key, out _);
            }
        }
        return await Task.FromResult(default(T));
    }

    public void Set(string key, T value)
    {
        lock (_lock)
        {
            var expirationTime = DateTime.Now.Add(_expirationTime);
            _cache[key] = new CacheItem<T>(value, expirationTime);
        }
    }

    public void Reset(string key)
    {
        if (_cache.ContainsKey(key))
        {
            _cache.TryRemove(key, out _);
        }
    }

    private class CacheItem<TValue>
    {
        public TValue Value { get; }
        public DateTime ExpirationTime { get; }

        public CacheItem(TValue value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }
}
