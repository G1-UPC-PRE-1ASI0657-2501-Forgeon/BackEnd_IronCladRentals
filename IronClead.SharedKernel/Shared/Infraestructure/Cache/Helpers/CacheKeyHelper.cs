using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache.Helpers;

/// <summary>
/// Helper para generar claves de cache
/// </summary>
public static class CacheKeyHelper
{
    /// <summary>
    /// Genera una clave de cache basada en el nombre del método y sus parámetros
    /// </summary>
    public static string GenerateKey(MethodInfo method, object[] args, string? prefix = null)
    {
        var className = method.DeclaringType?.Name ?? "Unknown";
        var methodName = method.Name;
        
        var keyBuilder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(prefix))
        {
            keyBuilder.Append($"{prefix}:");
        }
        
        keyBuilder.Append($"{className}:{methodName}");
        
        if (args?.Length > 0)
        {
            keyBuilder.Append(":");
            keyBuilder.Append(GenerateParameterHash(args));
        }
        
        return keyBuilder.ToString();
    }

    /// <summary>
    /// Genera una clave de cache personalizada
    /// </summary>
    public static string GenerateKey(string[] keyParts)
    {
        return string.Join(":", keyParts.Where(part => !string.IsNullOrEmpty(part)));
    }

    /// <summary>
    /// Genera una clave de cache para una entidad por ID
    /// </summary>
    public static string GenerateEntityKey<T>(object id, string? prefix = null)
    {
        var entityName = typeof(T).Name;
        var keyBuilder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(prefix))
        {
            keyBuilder.Append($"{prefix}:");
        }
        
        keyBuilder.Append($"{entityName}:{id}");
        
        return keyBuilder.ToString();
    }

    /// <summary>
    /// Genera una clave de cache para una lista de entidades
    /// </summary>
    public static string GenerateListKey<T>(string? suffix = null, string? prefix = null)
    {
        var entityName = typeof(T).Name;
        var keyBuilder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(prefix))
        {
            keyBuilder.Append($"{prefix}:");
        }
        
        keyBuilder.Append($"{entityName}:List");
        
        if (!string.IsNullOrEmpty(suffix))
        {
            keyBuilder.Append($":{suffix}");
        }
        
        return keyBuilder.ToString();
    }

    /// <summary>
    /// Genera un patrón para invalidar cache de una entidad
    /// </summary>
    public static string GenerateEntityPattern<T>(string? prefix = null)
    {
        var entityName = typeof(T).Name;
        var keyBuilder = new StringBuilder();
        
        if (!string.IsNullOrEmpty(prefix))
        {
            keyBuilder.Append($"{prefix}:");
        }
        
        keyBuilder.Append($"{entityName}:*");
        
        return keyBuilder.ToString();
    }

    /// <summary>
    /// Genera un hash para los parámetros del método
    /// </summary>
    private static string GenerateParameterHash(object[] args)
    {
        var combined = string.Join("|", args.Select(arg => 
        {
            if (arg == null) return "null";
            if (arg is string str) return str;
            if (arg.GetType().IsPrimitive) return arg.ToString();
            
            // Para objetos complejos, usar el hash code
            return arg.GetHashCode().ToString();
        }));
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToBase64String(hashBytes)[..8]; // Tomar solo los primeros 8 caracteres
    }

    /// <summary>
    /// Valida si una clave es válida
    /// </summary>
    public static bool IsValidKey(string key)
    {
        return !string.IsNullOrWhiteSpace(key) && 
               key.Length <= 250 && // Redis key limit
               !key.Contains(' ') &&
               !key.Contains('\n') &&
               !key.Contains('\r');
    }

    /// <summary>
    /// Sanitiza una clave removiendo caracteres no válidos
    /// </summary>
    public static string SanitizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return "unknown";
            
        var sanitized = key.Replace(" ", "_")
                          .Replace("\n", "")
                          .Replace("\r", "")
                          .Replace("\t", "_");
                          
        if (sanitized.Length > 250)
        {
            sanitized = sanitized.Substring(0, 250);
        }
        
        return sanitized;
    }
}
