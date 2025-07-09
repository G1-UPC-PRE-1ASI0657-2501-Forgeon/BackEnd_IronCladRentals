namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Enumeración para tipos de cache
/// </summary>
public enum CacheType
{
    Memory,
    Redis,
    Hybrid
}

/// <summary>
/// Configuración para el servicio de cache
/// </summary>
public class CacheConfiguration
{
    /// <summary>
    /// Tipo de cache a utilizar
    /// </summary>
    public CacheType CacheType { get; set; } = CacheType.Memory;

    /// <summary>
    /// Cadena de conexión para Redis (si se usa Redis)
    /// </summary>
    public string? RedisConnectionString { get; set; }

    /// <summary>
    /// Nombre de la instancia de Redis
    /// </summary>
    public string RedisInstanceName { get; set; } = "IronCleadRentals";

    /// <summary>
    /// Tiempo de expiración por defecto
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Tiempo de expiración por defecto para Memory Cache
    /// </summary>
    public TimeSpan MemoryCacheExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Tamaño máximo del Memory Cache (en MB)
    /// </summary>
    public int MemoryCacheSizeLimitMB { get; set; } = 100;

    /// <summary>
    /// Prefijo para las claves del cache
    /// </summary>
    public string KeyPrefix { get; set; } = "iron_clead:";

    /// <summary>
    /// Habilitar compresión para valores grandes
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Tamaño mínimo (en bytes) para aplicar compresión
    /// </summary>
    public int CompressionThreshold { get; set; } = 1024; // 1KB
}
