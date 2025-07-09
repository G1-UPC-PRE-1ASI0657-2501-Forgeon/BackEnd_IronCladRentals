using IronClead.SharedKernel.Shared.Infraestructure.Cache;
using IronClead.SharedKernel.Shared.Infraestructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IronClead.SharedKernel.Shared.Infraestructure.Extensions;

/// <summary>
/// Extensiones para configurar los servicios de cache
/// </summary>
public static class CacheServiceExtensions
{
    /// <summary>
    /// Agrega los servicios de cache al contenedor de dependencias
    /// </summary>
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configurar la configuración del cache
        var cacheConfig = new CacheConfiguration();
        configuration.GetSection("Cache").Bind(cacheConfig);
        services.AddSingleton(cacheConfig);

        // Registrar Memory Cache siempre (puede ser usado por el híbrido)
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = cacheConfig.MemoryCacheSizeLimitMB * 1024 * 1024; // Convertir MB a bytes
        });

        // Configurar servicios específicos según el tipo de cache
        switch (cacheConfig.CacheType)
        {
            case CacheType.Memory:
                services.AddSingleton<ICacheService>(provider =>
                {
                    var memoryCache = provider.GetRequiredService<IMemoryCache>();
                    var logger = provider.GetService<ILogger<MemoryCacheService>>();
                    return new MemoryCacheService(memoryCache, cacheConfig, logger);
                });
                break;

            case CacheType.Redis:
                services.AddRedisCache(cacheConfig);
                services.AddSingleton<ICacheService>(provider =>
                {
                    var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                    var logger = provider.GetService<ILogger<RedisCacheService>>();
                    return new RedisCacheService(connectionMultiplexer, cacheConfig, logger);
                });
                break;

            case CacheType.Hybrid:
                services.AddRedisCache(cacheConfig);
                services.AddSingleton<ICacheService>(provider =>
                {
                    var memoryCache = provider.GetRequiredService<IMemoryCache>();
                    var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                    
                    var memoryLogger = provider.GetService<ILogger<MemoryCacheService>>();
                    var redisLogger = provider.GetService<ILogger<RedisCacheService>>();
                    var hybridLogger = provider.GetService<ILogger<HybridCacheService>>();
                    
                    var memoryCacheService = new MemoryCacheService(memoryCache, cacheConfig, memoryLogger);
                    var redisCacheService = new RedisCacheService(connectionMultiplexer, cacheConfig, redisLogger);
                    
                    return new HybridCacheService(memoryCacheService, redisCacheService, cacheConfig, hybridLogger);
                });
                break;

            default:
                throw new NotSupportedException($"Cache type {cacheConfig.CacheType} is not supported");
        }

        // Registrar el factory
        services.AddSingleton<CacheServiceFactory>();

        return services;
    }

    /// <summary>
    /// Configura Redis si es necesario
    /// </summary>
    private static IServiceCollection AddRedisCache(this IServiceCollection services, CacheConfiguration cacheConfig)
    {
        if (string.IsNullOrEmpty(cacheConfig.RedisConnectionString))
        {
            throw new InvalidOperationException("Redis connection string is required");
        }

        // Registrar Redis connection
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse(cacheConfig.RedisConnectionString);
            configuration.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(configuration);
        });

        // Registrar Redis distributed cache (opcional, para compatibilidad con IDistributedCache)
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cacheConfig.RedisConnectionString;
            options.InstanceName = cacheConfig.RedisInstanceName;
        });

        return services;
    }

    /// <summary>
    /// Agrega cache services con configuración personalizada
    /// </summary>
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services,
        Action<CacheConfiguration> configureOptions)
    {
        var cacheConfig = new CacheConfiguration();
        configureOptions(cacheConfig);
        
        services.AddSingleton(cacheConfig);
        
        return services.AddCacheServices(cacheConfig);
    }

    /// <summary>
    /// Agrega cache services con configuración directa
    /// </summary>
    private static IServiceCollection AddCacheServices(
        this IServiceCollection services,
        CacheConfiguration cacheConfig)
    {
        // Registrar Memory Cache siempre
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = cacheConfig.MemoryCacheSizeLimitMB * 1024 * 1024;
        });

        // Configurar según el tipo
        switch (cacheConfig.CacheType)
        {
            case CacheType.Memory:
                services.AddSingleton<ICacheService>(provider =>
                {
                    var memoryCache = provider.GetRequiredService<IMemoryCache>();
                    var logger = provider.GetService<ILogger<MemoryCacheService>>();
                    return new MemoryCacheService(memoryCache, cacheConfig, logger);
                });
                break;

            case CacheType.Redis:
                services.AddRedisCache(cacheConfig);
                services.AddSingleton<ICacheService>(provider =>
                {
                    var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                    var logger = provider.GetService<ILogger<RedisCacheService>>();
                    return new RedisCacheService(connectionMultiplexer, cacheConfig, logger);
                });
                break;

            case CacheType.Hybrid:
                services.AddRedisCache(cacheConfig);
                services.AddSingleton<ICacheService>(provider =>
                {
                    var memoryCache = provider.GetRequiredService<IMemoryCache>();
                    var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
                    
                    var memoryLogger = provider.GetService<ILogger<MemoryCacheService>>();
                    var redisLogger = provider.GetService<ILogger<RedisCacheService>>();
                    var hybridLogger = provider.GetService<ILogger<HybridCacheService>>();
                    
                    var memoryCacheService = new MemoryCacheService(memoryCache, cacheConfig, memoryLogger);
                    var redisCacheService = new RedisCacheService(connectionMultiplexer, cacheConfig, redisLogger);
                    
                    return new HybridCacheService(memoryCacheService, redisCacheService, cacheConfig, hybridLogger);
                });
                break;
        }

        services.AddSingleton<CacheServiceFactory>();
        return services;
    }
}
