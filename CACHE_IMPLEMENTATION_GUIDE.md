# Sistema de Cache - IronClead Rentals

Este documento describe cómo implementar y usar el sistema de cache en el proyecto IronClead Rentals.

## Características

- **Múltiples Proveedores**: Memory Cache, Redis, y Híbrido (Memory + Redis)
- **Serialización Automática**: Conversión automática de objetos a JSON
- **Compresión**: Compresión automática para objetos grandes
- **Generación de Claves**: Helpers para generar claves consistentes
- **Invalidación Inteligente**: Soporte para patrones de invalidación
- **Logging**: Logging integrado para monitoreo
- **Configuración Flexible**: Configuración a través de appsettings.json

## Configuración

### 1. appsettings.json

```json
{
  "Cache": {
    "CacheType": "Memory", // "Memory", "Redis", "Hybrid"
    "RedisConnectionString": "localhost:6379",
    "RedisInstanceName": "IronCleadRentals",
    "DefaultExpiration": "00:30:00",
    "MemoryCacheExpiration": "00:15:00",
    "MemoryCacheSizeLimitMB": 100,
    "KeyPrefix": "iron_clead:",
    "EnableCompression": true,
    "CompressionThreshold": 1024
  }
}
```

### 2. Program.cs

```csharp
using IronClead.SharedKernel.Shared.Infraestructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de cache
builder.Services.AddCacheServices(builder.Configuration);

var app = builder.Build();
```

## Tipos de Cache

### Memory Cache
- **Ventajas**: Extremadamente rápido, no requiere infraestructura externa
- **Desventajas**: Se pierde al reiniciar la aplicación, no compartido entre instancias
- **Uso recomendado**: Desarrollo local, aplicaciones de una sola instancia

### Redis Cache
- **Ventajas**: Persistente, compartido entre instancias, escalable
- **Desventajas**: Latencia de red, requiere servidor Redis
- **Uso recomendado**: Producción, aplicaciones distribuidas

### Híbrido (Memory + Redis)
- **Ventajas**: Combina velocidad de Memory con persistencia de Redis
- **Desventajas**: Mayor complejidad, uso de más recursos
- **Uso recomendado**: Aplicaciones de alto rendimiento

## Uso Básico

### Inyección de Dependencias

```csharp
public class VehicleService
{
    private readonly ICacheService _cacheService;
    
    public VehicleService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
}
```

### Operaciones de Cache

```csharp
// Obtener del cache
var vehicle = await _cacheService.GetAsync<Vehicle>("vehicle_123");

// Establecer en cache
await _cacheService.SetAsync("vehicle_123", vehicle, TimeSpan.FromHours(1));

// Verificar existencia
var exists = await _cacheService.ExistsAsync("vehicle_123");

// Remover del cache
await _cacheService.RemoveAsync("vehicle_123");

// Remover por patrón
await _cacheService.RemoveByPatternAsync("vehicle_*");
```

## Generación de Claves

### Usando CacheKeyHelper

```csharp
// Clave para entidad por ID
var key = CacheKeyHelper.GenerateEntityKey<Vehicle>(vehicleId);

// Clave para lista
var listKey = CacheKeyHelper.GenerateListKey<Vehicle>("available");

// Clave personalizada
var customKey = CacheKeyHelper.GenerateKey(new[] { "vehicles", "company", companyId.ToString() });

// Patrón para invalidación
var pattern = CacheKeyHelper.GenerateEntityPattern<Vehicle>();
```

## Patrones de Implementación

### 1. Cache-Aside (Read-Through)

```csharp
public async Task<Vehicle?> GetVehicleAsync(int id)
{
    var cacheKey = CacheKeyHelper.GenerateEntityKey<Vehicle>(id);
    
    // Intentar obtener del cache
    var cached = await _cacheService.GetAsync<Vehicle>(cacheKey);
    if (cached != null)
        return cached;
    
    // Obtener de la base de datos
    var vehicle = await _repository.GetByIdAsync(id);
    if (vehicle != null)
    {
        // Almacenar en cache
        await _cacheService.SetAsync(cacheKey, vehicle, TimeSpan.FromHours(1));
    }
    
    return vehicle;
}
```

### 2. Write-Through (Invalidación)

```csharp
public async Task<Vehicle> UpdateVehicleAsync(int id, Vehicle vehicle)
{
    // Actualizar en base de datos
    var updated = await _repository.UpdateAsync(id, vehicle);
    
    // Invalidar cache relacionado
    var cacheKey = CacheKeyHelper.GenerateEntityKey<Vehicle>(id);
    await _cacheService.RemoveAsync(cacheKey);
    
    // Invalidar listas relacionadas
    await _cacheService.RemoveByPatternAsync("vehicles_by_company_*");
    
    return updated;
}
```

### 3. Cache con Fallback

```csharp
public async Task<List<Vehicle>> GetAvailableVehiclesAsync(DateTime start, DateTime end)
{
    var cacheKey = $"available_vehicles_{start:yyyy-MM-dd}_{end:yyyy-MM-dd}";
    
    try
    {
        var cached = await _cacheService.GetAsync<List<Vehicle>>(cacheKey);
        if (cached != null)
            return cached;
            
        var vehicles = await _repository.GetAvailableAsync(start, end);
        await _cacheService.SetAsync(cacheKey, vehicles, TimeSpan.FromMinutes(15));
        
        return vehicles;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Cache error, falling back to database");
        return await _repository.GetAvailableAsync(start, end);
    }
}
```

## Estrategias de Invalidación

### Por Entidad
```csharp
// Invalidar vehículo específico
await _cacheService.RemoveAsync(CacheKeyHelper.GenerateEntityKey<Vehicle>(vehicleId));
```

### Por Patrón
```csharp
// Invalidar todos los vehículos
await _cacheService.RemoveByPatternAsync(CacheKeyHelper.GenerateEntityPattern<Vehicle>());

// Invalidar vehículos de una empresa
await _cacheService.RemoveByPatternAsync($"*vehicles_by_company_{companyId}*");
```

### Invalidación en Cascada
```csharp
private async Task InvalidateVehicleCacheAsync(int vehicleId, int companyId)
{
    var tasks = new List<Task>
    {
        _cacheService.RemoveAsync(CacheKeyHelper.GenerateEntityKey<Vehicle>(vehicleId)),
        _cacheService.RemoveByPatternAsync($"*vehicles_by_company_{companyId}*"),
        _cacheService.RemoveByPatternAsync("*available_vehicles*")
    };
    
    await Task.WhenAll(tasks);
}
```

## Mejores Prácticas

### 1. Tiempos de Expiración
- **Datos estáticos**: 1-24 horas
- **Datos semi-dinámicos**: 15-60 minutos
- **Datos dinámicos**: 1-15 minutos

### 2. Manejo de Errores
- Siempre tener un fallback a la fuente de datos
- Logear errores de cache pero no fallar la operación
- Considerar cache stale como válido en errores

### 3. Claves de Cache
- Usar prefijos consistentes
- Incluir versión de la aplicación si es necesario
- Mantener claves descriptivas pero cortas

### 4. Monitoreo
- Logear hit/miss ratios
- Monitorear uso de memoria
- Alertar en errores de conexión a Redis

## Configuración para Diferentes Entornos

### Desarrollo
```json
{
  "Cache": {
    "CacheType": "Memory",
    "MemoryCacheExpiration": "00:05:00",
    "MemoryCacheSizeLimitMB": 50
  }
}
```

### Producción
```json
{
  "Cache": {
    "CacheType": "Redis",
    "RedisConnectionString": "your-redis-server:6379",
    "DefaultExpiration": "01:00:00",
    "EnableCompression": true
  }
}
```

### Alta Disponibilidad
```json
{
  "Cache": {
    "CacheType": "Hybrid",
    "RedisConnectionString": "your-redis-cluster:6379",
    "MemoryCacheExpiration": "00:10:00",
    "DefaultExpiration": "02:00:00"
  }
}
```

## Troubleshooting

### Problemas Comunes

1. **Cache Miss Frecuente**
   - Verificar configuración de expiración
   - Revisar patrones de invalidación
   - Confirmar generación de claves

2. **Errores de Serialización**
   - Verificar que las clases sean serializables
   - Revisar dependencias circulares
   - Considerar DTOs para cache

3. **Memoria Alta**
   - Ajustar límites de Memory Cache
   - Revisar tiempos de expiración
   - Implementar compresión

4. **Conexión Redis**
   - Verificar string de conexión
   - Confirmar conectividad de red
   - Revisar configuración de firewall

### Logs Importantes

```csharp
_logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
_logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
_logger.LogWarning("Cache operation failed for key: {CacheKey}", cacheKey);
_logger.LogError(ex, "Redis connection error");
```

## Extensiones Futuras

- Cache distribuido con invalidación por eventos
- Métricas automáticas de rendimiento
- Cache warming automático
- Soporte para cache asíncrono por lotes
- Integración con APM tools
