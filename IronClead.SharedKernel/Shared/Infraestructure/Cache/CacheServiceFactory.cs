using IronClead.SharedKernel.Shared.Infraestructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Factory para crear instancias del servicio de cache
/// </summary>
public class CacheServiceFactory
{
    private readonly CacheConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory? _loggerFactory;

    public CacheServiceFactory(
        CacheConfiguration configuration, 
        IServiceProvider serviceProvider,
        ILoggerFactory? loggerFactory = null)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Crea una instancia del servicio de cache basado en la configuración
    /// </summary>
    public ICacheService CreateCacheService()
    {
        return _configuration.CacheType switch
        {
            CacheType.Memory => CreateMemoryCacheService(),
            CacheType.Redis => CreateRedisCacheService(),
            CacheType.Hybrid => CreateHybridCacheService(),
            _ => throw new NotSupportedException($"Cache type {_configuration.CacheType} is not supported")
        };
    }

    private ICacheService CreateMemoryCacheService()
    {
        var memoryCache = _serviceProvider.GetService(typeof(IMemoryCache)) as IMemoryCache;
        if (memoryCache == null)
        {
            throw new InvalidOperationException("IMemoryCache service is not registered");
        }

        var logger = _loggerFactory?.CreateLogger<MemoryCacheService>();
        return new MemoryCacheService(memoryCache, _configuration, logger);
    }

    private ICacheService CreateRedisCacheService()
    {
        if (string.IsNullOrEmpty(_configuration.RedisConnectionString))
        {
            throw new InvalidOperationException("Redis connection string is required for Redis cache");
        }

        var connectionMultiplexer = _serviceProvider.GetService(typeof(IConnectionMultiplexer)) as IConnectionMultiplexer;
        if (connectionMultiplexer == null)
        {
            throw new InvalidOperationException("IConnectionMultiplexer service is not registered");
        }

        var logger = _loggerFactory?.CreateLogger<RedisCacheService>();
        return new RedisCacheService(connectionMultiplexer, _configuration, logger);
    }

    private ICacheService CreateHybridCacheService()
    {
        var memoryService = CreateMemoryCacheService() as MemoryCacheService;
        var redisService = CreateRedisCacheService() as RedisCacheService;

        if (memoryService == null || redisService == null)
        {
            throw new InvalidOperationException("Unable to create Memory or Redis cache services for hybrid cache");
        }

        var logger = _loggerFactory?.CreateLogger<HybridCacheService>();
        return new HybridCacheService(memoryService, redisService, _configuration, logger);
    }
}
