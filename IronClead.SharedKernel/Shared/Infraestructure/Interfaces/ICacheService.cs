using System;
using System.Threading.Tasks;

namespace IronClead.SharedKernel.Shared.Infraestructure.Interfaces;

/// <summary>
/// Interface para el servicio de cache genérico
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Obtiene un valor del cache de forma asíncrona
    /// </summary>
    /// <typeparam name="T">Tipo del valor a obtener</typeparam>
    /// <param name="key">Clave del cache</param>
    /// <returns>El valor cacheado o null si no existe</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// Establece un valor en el cache de forma asíncrona
    /// </summary>
    /// <typeparam name="T">Tipo del valor a cachear</typeparam>
    /// <param name="key">Clave del cache</param>
    /// <param name="value">Valor a cachear</param>
    /// <param name="expiration">Tiempo de expiración (opcional)</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Remueve un valor del cache de forma asíncrona
    /// </summary>
    /// <param name="key">Clave del cache a remover</param>
    Task RemoveAsync(string key);

    /// <summary>
    /// Remueve múltiples valores del cache que coincidan con un patrón
    /// </summary>
    /// <param name="pattern">Patrón para buscar las claves</param>
    Task RemoveByPatternAsync(string pattern);

    /// <summary>
    /// Verifica si existe una clave en el cache
    /// </summary>
    /// <param name="key">Clave a verificar</param>
    /// <returns>True si existe, false si no</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// Obtiene un valor del cache de forma síncrona
    /// </summary>
    /// <typeparam name="T">Tipo del valor a obtener</typeparam>
    /// <param name="key">Clave del cache</param>
    /// <returns>El valor cacheado o null si no existe</returns>
    T? Get<T>(string key) where T : class;

    /// <summary>
    /// Establece un valor en el cache de forma síncrona
    /// </summary>
    /// <typeparam name="T">Tipo del valor a cachear</typeparam>
    /// <param name="key">Clave del cache</param>
    /// <param name="value">Valor a cachear</param>
    /// <param name="expiration">Tiempo de expiración (opcional)</param>
    void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// Remueve un valor del cache de forma síncrona
    /// </summary>
    /// <param name="key">Clave del cache a remover</param>
    void Remove(string key);
}
