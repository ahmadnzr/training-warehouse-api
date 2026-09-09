using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace WarehouseWeb.Api.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MemoryCacheService> _logger;
        private static readonly ConcurrentDictionary<string, bool> _trackedKeys = new();

        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
            {
                _logger.LogDebug("Cache HIT for key: {CacheKey}", key);
                return Task.FromResult<T?>(typedValue);
            }

            _logger.LogDebug("Cache MISS for key: {CacheKey}", key);
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            options.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
            {
                _trackedKeys.TryRemove(evictedKey.ToString() ?? string.Empty, out _);
            });

            _memoryCache.Set(key, value, options);
            _trackedKeys.TryAdd(key, true);
            return Task.CompletedTask;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null)
            {
                return cached;
            }

            var result = await factory();
            if (result != null)
            {
                await SetAsync(key, result, expiration);
            }

            return result;
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            _trackedKeys.TryRemove(key, out _);
            _logger.LogDebug("Cache removed for key: {CacheKey}", key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            var matchedKeys = _trackedKeys.Keys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in matchedKeys)
            {
                _memoryCache.Remove(key);
                _trackedKeys.TryRemove(key, out _);
                _logger.LogDebug("Cache removed by prefix [{Prefix}]: {CacheKey}", prefix, key);
            }

            return Task.CompletedTask;
        }
    }
}
