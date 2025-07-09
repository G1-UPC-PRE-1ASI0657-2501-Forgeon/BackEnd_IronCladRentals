using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Implementación del servicio de cache usando Memory Cache
/// </summary>
public class MemoryCacheService : CacheServiceBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService>? _logger;
    private readonly ConcurrentDictionary<string, bool> _keys;

    public MemoryCacheService(
        IMemoryCache memoryCache, 
        CacheConfiguration configuration,
        ILogger<MemoryCacheService>? logger = null) 
        : base(configuration)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _keys = new ConcurrentDictionary<string, bool>();
    }

    public override async Task<T?> GetAsync<T>(string key) where T : class
    {
        return await Task.FromResult(Get<T>(key));
    }

    public override async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        await Task.Run(() => Set(key, value, expiration));
    }

    public override async Task RemoveAsync(string key)
    {
        await Task.Run(() => Remove(key));
    }

    public override async Task RemoveByPatternAsync(string pattern)
    {
        await Task.Run(() =>
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);
                var keysToRemove = _keys.Keys.Where(k => regex.IsMatch(k)).ToList();
                
                foreach (var key in keysToRemove)
                {
                    Remove(key);
                }
                
                _logger?.LogInformation($"Removed {keysToRemove.Count} cache entries matching pattern: {pattern}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error removing cache entries by pattern: {pattern}");
            }
        });
    }

    public override async Task<bool> ExistsAsync(string key)
    {
        return await Task.FromResult(_memoryCache.TryGetValue(GenerateKey(key), out _));
    }

    public override T? Get<T>(string key) where T : class
    {
        try
        {
            var fullKey = GenerateKey(key);
            if (_memoryCache.TryGetValue(fullKey, out var cachedValue) && cachedValue is string serializedValue)
            {
                return Deserialize<T>(serializedValue);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error getting cache value for key: {key}");
            return null;
        }
    }

    public override void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (value == null) return;

            var fullKey = GenerateKey(key);
            var serializedValue = Serialize(value);
            var effectiveExpiration = expiration ?? _configuration.MemoryCacheExpiration;

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = effectiveExpiration,
                Size = serializedValue.Length,
                PostEvictionCallbacks = 
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = (k, v, reason, state) =>
                        {
                            if (k is string keyStr)
                            {
                                _keys.TryRemove(keyStr, out _);
                            }
                        }
                    }
                }
            };

            _memoryCache.Set(fullKey, serializedValue, cacheEntryOptions);
            _keys.TryAdd(fullKey, true);
            
            _logger?.LogDebug($"Cache value set for key: {key}, expiration: {effectiveExpiration}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error setting cache value for key: {key}");
        }
    }

    public override void Remove(string key)
    {
        try
        {
            var fullKey = GenerateKey(key);
            _memoryCache.Remove(fullKey);
            _keys.TryRemove(fullKey, out _);
            
            _logger?.LogDebug($"Cache value removed for key: {key}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error removing cache value for key: {key}");
        }
    }
}
