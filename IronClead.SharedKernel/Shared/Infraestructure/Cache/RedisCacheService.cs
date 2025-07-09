using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Implementación del servicio de cache usando Redis
/// </summary>
public class RedisCacheService : CacheServiceBase
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService>? _logger;

    public RedisCacheService(
        IConnectionMultiplexer connectionMultiplexer,
        CacheConfiguration configuration,
        ILogger<RedisCacheService>? logger = null) 
        : base(configuration)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _database = connectionMultiplexer.GetDatabase();
        _logger = logger;
    }

    public override async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var fullKey = GenerateKey(key);
            var value = await _database.StringGetAsync(fullKey);
            
            if (!value.HasValue)
                return null;

            return Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error getting cache value for key: {key}");
            return null;
        }
    }

    public override async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (value == null) return;

            var fullKey = GenerateKey(key);
            var serializedValue = Serialize(value);
            var effectiveExpiration = expiration ?? _configuration.DefaultExpiration;

            await _database.StringSetAsync(fullKey, serializedValue, effectiveExpiration);
            
            _logger?.LogDebug($"Cache value set for key: {key}, expiration: {effectiveExpiration}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error setting cache value for key: {key}");
        }
    }

    public override async Task RemoveAsync(string key)
    {
        try
        {
            var fullKey = GenerateKey(key);
            await _database.KeyDeleteAsync(fullKey);
            
            _logger?.LogDebug($"Cache value removed for key: {key}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error removing cache value for key: {key}");
        }
    }

    public override async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: GenerateKey(pattern)).ToArray();
            
            if (keys.Any())
            {
                await _database.KeyDeleteAsync(keys);
                _logger?.LogInformation($"Removed {keys.Length} cache entries matching pattern: {pattern}");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error removing cache entries by pattern: {pattern}");
        }
    }

    public override async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var fullKey = GenerateKey(key);
            return await _database.KeyExistsAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error checking cache key existence: {key}");
            return false;
        }
    }

    public override T? Get<T>(string key) where T : class
    {
        try
        {
            var fullKey = GenerateKey(key);
            var value = _database.StringGet(fullKey);
            
            if (!value.HasValue)
                return null;

            return Deserialize<T>(value!);
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
            var effectiveExpiration = expiration ?? _configuration.DefaultExpiration;

            _database.StringSet(fullKey, serializedValue, effectiveExpiration);
            
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
            _database.KeyDelete(fullKey);
            
            _logger?.LogDebug($"Cache value removed for key: {key}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error removing cache value for key: {key}");
        }
    }
}
