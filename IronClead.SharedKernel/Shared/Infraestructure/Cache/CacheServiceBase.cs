using System.IO.Compression;
using System.Text;
using IronClead.SharedKernel.Shared.Infraestructure.Interfaces;
using Newtonsoft.Json;

namespace IronClead.SharedKernel.Shared.Infraestructure.Cache;

/// <summary>
/// Clase base abstracta para servicios de cache
/// </summary>
public abstract class CacheServiceBase : ICacheService
{
    protected readonly CacheConfiguration _configuration;
    private readonly JsonSerializerSettings _jsonSettings;

    protected CacheServiceBase(CacheConfiguration configuration)
    {
        _configuration = configuration;
        _jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // ✅ Ignorar bucles
            MaxDepth = 32, // ✅ Limitar profundidad
            PreserveReferencesHandling = PreserveReferencesHandling.Objects // ✅ Preservar referencias
        };
    }

    /// <summary>
    /// Genera la clave completa con prefijo
    /// </summary>
    protected string GenerateKey(string key)
    {
        return $"{_configuration.KeyPrefix}{key}";
    }

    /// <summary>
    /// Serializa un objeto a string
    /// </summary>
    protected string Serialize<T>(T value) where T : class
    {
        var json = JsonConvert.SerializeObject(value, _jsonSettings);
        
        if (_configuration.EnableCompression && 
            Encoding.UTF8.GetByteCount(json) > _configuration.CompressionThreshold)
        {
            return CompressString(json);
        }
        
        return json;
    }

    /// <summary>
    /// Deserializa un string a objeto
    /// </summary>
    protected T? Deserialize<T>(string value) where T : class
    {
        try
        {
            // Verificar si está comprimido (simple heurística)
            var decompressedValue = value;
            if (IsCompressed(value))
            {
                decompressedValue = DecompressString(value);
            }
            
            return JsonConvert.DeserializeObject<T>(decompressedValue, _jsonSettings);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Comprime una cadena usando GZip
    /// </summary>
    private string CompressString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            gzipStream.Write(bytes, 0, bytes.Length);
        }
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    /// <summary>
    /// Descomprime una cadena usando GZip
    /// </summary>
    private string DecompressString(string compressedText)
    {
        var gzipBytes = Convert.FromBase64String(compressedText);
        using var memoryStream = new MemoryStream(gzipBytes);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Verifica si una cadena está comprimida (heurística simple)
    /// </summary>
    private bool IsCompressed(string value)
    {
        try
        {
            Convert.FromBase64String(value);
            return !value.TrimStart().StartsWith("{") && !value.TrimStart().StartsWith("[");
        }
        catch
        {
            return false;
        }
    }

    // Métodos abstractos que deben ser implementados por las clases derivadas
    public abstract Task<T?> GetAsync<T>(string key) where T : class;
    public abstract Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    public abstract Task RemoveAsync(string key);
    public abstract Task RemoveByPatternAsync(string pattern);
    public abstract Task<bool> ExistsAsync(string key);
    public abstract T? Get<T>(string key) where T : class;
    public abstract void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    public abstract void Remove(string key);
}
