namespace IronClead.SharedKernel.Shared.Infraestructure.Cache.Attributes;

/// <summary>
/// Atributo para marcar métodos que deben ser cacheados automáticamente
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CacheableAttribute : Attribute
{
    /// <summary>
    /// Clave del cache. Si no se especifica, se genera automáticamente
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Tiempo de expiración en minutos
    /// </summary>
    public int ExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Prefijo adicional para la clave
    /// </summary>
    public string? KeyPrefix { get; set; }

    /// <summary>
    /// Parámetros a incluir en la generación automática de la clave
    /// </summary>
    public string[]? KeyParameters { get; set; }

    /// <summary>
    /// Si debe invalidar el cache en lugar de obtener/establecer
    /// </summary>
    public bool InvalidateCache { get; set; } = false;

    /// <summary>
    /// Patrón para invalidar múltiples entradas de cache
    /// </summary>
    public string? InvalidationPattern { get; set; }

    public TimeSpan Expiration => TimeSpan.FromMinutes(ExpirationMinutes);
}
