using Microsoft.Extensions.Logging;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Implementación híbrida del servicio de cache (Memory + Redis)
/// Utiliza Memory Cache como L1 y Redis como L2
/// </summary>
public class HybridCacheService : CacheServiceBase
{
    private readonly MemoryCacheService _memoryCache;
    private readonly RedisCacheService _redisCache;
    private readonly ILogger<HybridCacheService>? _logger;

    public HybridCacheService(
        MemoryCacheService memoryCache,
        RedisCacheService redisCache,
        CacheConfiguration configuration,
        ILogger<HybridCacheService>? logger = null) 
        : base(configuration)
    {
        _memoryCache = memoryCache;
        _redisCache = redisCache;
        _logger = logger;
    }

    public override async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            // Intentar obtener del Memory Cache primero (L1)
            var value = await _memoryCache.GetAsync<T>(key);
            if (value != null)
            {
                _logger?.LogDebug($"Cache hit in Memory for key: {key}");
                return value;
            }

            // Si no está en Memory Cache, intentar Redis (L2)
            value = await _redisCache.GetAsync<T>(key);
            if (value != null)
            {
                _logger?.LogDebug($"Cache hit in Redis for key: {key}");
                
                // Almacenar en Memory Cache para futuras consultas
                await _memoryCache.SetAsync(key, value, _configuration.MemoryCacheExpiration);
                return value;
            }

            _logger?.LogDebug($"Cache miss for key: {key}");
            return null;
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

            // Establecer en ambos caches
            var tasks = new List<Task>
            {
                _memoryCache.SetAsync(key, value, _configuration.MemoryCacheExpiration),
                _redisCache.SetAsync(key, value, expiration)
            };

            await Task.WhenAll(tasks);
            _logger?.LogDebug($"Cache value set in both Memory and Redis for key: {key}");
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
            // Remover de ambos caches
            var tasks = new List<Task>
            {
                _memoryCache.RemoveAsync(key),
                _redisCache.RemoveAsync(key)
            };

            await Task.WhenAll(tasks);
            _logger?.LogDebug($"Cache value removed from both Memory and Redis for key: {key}");
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
            // Remover de ambos caches
            var tasks = new List<Task>
            {
                _memoryCache.RemoveByPatternAsync(pattern),
                _redisCache.RemoveByPatternAsync(pattern)
            };

            await Task.WhenAll(tasks);
            _logger?.LogInformation($"Cache entries removed from both Memory and Redis matching pattern: {pattern}");
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
            // Verificar primero en Memory Cache
            var existsInMemory = await _memoryCache.ExistsAsync(key);
            if (existsInMemory)
                return true;

            // Si no está en Memory, verificar en Redis
            return await _redisCache.ExistsAsync(key);
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
            // Intentar obtener del Memory Cache primero (L1)
            var value = _memoryCache.Get<T>(key);
            if (value != null)
            {
                _logger?.LogDebug($"Cache hit in Memory for key: {key}");
                return value;
            }

            // Si no está en Memory Cache, intentar Redis (L2)
            value = _redisCache.Get<T>(key);
            if (value != null)
            {
                _logger?.LogDebug($"Cache hit in Redis for key: {key}");
                
                // Almacenar en Memory Cache para futuras consultas
                _memoryCache.Set(key, value, _configuration.MemoryCacheExpiration);
                return value;
            }

            _logger?.LogDebug($"Cache miss for key: {key}");
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

            // Establecer en ambos caches
            _memoryCache.Set(key, value, _configuration.MemoryCacheExpiration);
            _redisCache.Set(key, value, expiration);
            
            _logger?.LogDebug($"Cache value set in both Memory and Redis for key: {key}");
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
            // Remover de ambos caches
            _memoryCache.Remove(key);
            _redisCache.Remove(key);
            
            _logger?.LogDebug($"Cache value removed from both Memory and Redis for key: {key}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, $"Error removing cache value for key: {key}");
        }
    }
}
