using IronClead.SharedKernel.Shared.Infraestructure.Cache.Helpers;
using IronClead.SharedKernel.Shared.Infraestructure.Interfaces;
using Microsoft.Extensions.Logging;
using VehicleService.VehicleBounded.Application.DTOs.Cache;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class VehicleQueryService : IVehicleQueryService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<VehicleQueryService> _logger;

    public VehicleQueryService(
        IVehicleRepository vehicleRepository,
        ICacheService cacheService,
        ILogger<VehicleQueryService> logger)
    {
        _vehicleRepository = vehicleRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate, string? city = null)
    {
        // Generar clave de cache única para esta consulta
        var cacheKey = CacheKeyHelper.GenerateKey(new[]
        {
            "available_vehicles",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"),
            city ?? "all_cities"
        });

        try
        {
            // Intentar obtener del cache primero
            var cachedVehicles = await _cacheService.GetAsync<List<VehicleCacheDto>>(cacheKey);
            if (cachedVehicles != null)
            {
                _logger.LogInformation($"Cache hit for available vehicles query: {cacheKey}");
                // Para mantener la interfaz, convertimos de vuelta a entidades básicas
                // En el controller se podrían usar directamente los DTOs
                return ConvertFromCacheDto(cachedVehicles);
            }

            // Si no está en cache, obtener de la base de datos
            _logger.LogInformation($"Cache miss for available vehicles query: {cacheKey}");
            var availableVehicles = await _vehicleRepository.SearchAvailableVehiclesAsync(startDate, endDate);
            var vehicleList = availableVehicles.ToList();

            // Convertir a DTOs para cache
            var vehicleDtos = ConvertToCacheDto(vehicleList);
            
            // Almacenar en cache por 15 minutos
            await _cacheService.SetAsync(cacheKey, vehicleDtos, TimeSpan.FromMinutes(15));
            
            return vehicleList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetAvailableVehiclesAsync with cache key: {cacheKey}");
            // Si hay error con cache, obtener directamente de la base de datos
            return await _vehicleRepository.SearchAvailableVehiclesAsync(startDate, endDate);
        }
    }

    /// <summary>
    /// Método adicional para obtener DTOs directamente del cache (para usar en controllers)
    /// </summary>
    public async Task<IEnumerable<VehicleCacheDto>?> GetAvailableVehiclesDtoAsync(DateTime startDate, DateTime endDate, string? city = null)
    {
        var cacheKey = CacheKeyHelper.GenerateKey(new[]
        {
            "available_vehicles",
            startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"),
            city ?? "all_cities"
        });

        try
        {
            // Intentar obtener del cache primero
            var cachedVehicles = await _cacheService.GetAsync<List<VehicleCacheDto>>(cacheKey);
            if (cachedVehicles != null)
            {
                _logger.LogInformation($"Cache hit for available vehicles DTO query: {cacheKey}");
                return cachedVehicles;
            }

            // Si no está en cache, obtener de la base de datos y cachear
            _logger.LogInformation($"Cache miss for available vehicles DTO query: {cacheKey}");
            var availableVehicles = await _vehicleRepository.SearchAvailableVehiclesAsync(startDate, endDate);
            var vehicleList = availableVehicles.ToList();

            // Convertir a DTOs para cache
            var vehicleDtos = ConvertToCacheDto(vehicleList);
            
            // Almacenar en cache por 15 minutos
            await _cacheService.SetAsync(cacheKey, vehicleDtos, TimeSpan.FromMinutes(15));
            
            return vehicleDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetAvailableVehiclesDtoAsync with cache key: {cacheKey}");
            return null;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByCompanyIdAsync(int companyId)
    {
        var cacheKey = CacheKeyHelper.GenerateKey(new[] { "vehicles_by_company", companyId.ToString() });

        try
        {
            var cachedVehicles = await _cacheService.GetAsync<List<VehicleCacheDto>>(cacheKey);
            if (cachedVehicles != null)
            {
                _logger.LogInformation($"Cache hit for vehicles by company query: {cacheKey}");
                return ConvertFromCacheDto(cachedVehicles);
            }

            _logger.LogInformation($"Cache miss for vehicles by company query: {cacheKey}");
            var vehicles = await _vehicleRepository.GetByCompanyIdAsync(companyId);
            var vehicleList = vehicles.ToList();

            // Convertir a DTOs para cache
            var vehicleDtos = ConvertToCacheDto(vehicleList);
            
            // Almacenar en cache por 30 minutos
            await _cacheService.SetAsync(cacheKey, vehicleDtos, TimeSpan.FromMinutes(30));
            
            return vehicleList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetVehiclesByCompanyIdAsync with cache key: {cacheKey}");
            return await _vehicleRepository.GetByCompanyIdAsync(companyId);
        }
    }

    public async Task<Vehicle?> GetVehicleDetailsAsync(int vehicleId)
    {
        var cacheKey = CacheKeyHelper.GenerateEntityKey<Vehicle>(vehicleId);

        try
        {
            var cachedVehicle = await _cacheService.GetAsync<VehicleCacheDto>(cacheKey);
            if (cachedVehicle != null)
            {
                _logger.LogInformation($"Cache hit for vehicle details: {cacheKey}");
                // Para detalles específicos, mejor obtener de DB para datos completos
                return await _vehicleRepository.GetByIdAsync(vehicleId);
            }

            _logger.LogInformation($"Cache miss for vehicle details: {cacheKey}");
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            
            if (vehicle != null)
            {
                // Convertir a DTO para cache
                var vehicleDto = ConvertToCacheDto(new[] { vehicle }).First();
                
                // Almacenar en cache por 1 hora
                await _cacheService.SetAsync(cacheKey, vehicleDto, TimeSpan.FromHours(1));
            }
            
            return vehicle;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GetVehicleDetailsAsync with cache key: {cacheKey}");
            return await _vehicleRepository.GetByIdAsync(vehicleId);
        }
    }

    /// <summary>
    /// Convierte entidades Vehicle a DTOs para cache
    /// </summary>
    private List<VehicleCacheDto> ConvertToCacheDto(IEnumerable<Vehicle> vehicles)
    {
        return vehicles.Select(v => new VehicleCacheDto
        {
            Id = v.Id,
            Passengers = v.Passengers,
            LuggageCapacity = v.LuggageCapacity,
            LicensePlate = v.LicensePlate,
            Color = v.Color,
            Year = v.Year,
            Transmission = v.Transmission,
            FuelType = v.FuelType,
            ImageUrl = v.ImageUrl,
            ModelId = v.ModelId,
            ModelName = v.Model?.CarModel ?? "",
            BrandId = v.BrandId,
            BrandName = v.Brand?.BrandName ?? "",
            CompanyId = v.CompanyId,
            CompanyName = v.Company?.Name ?? "",
            CompanyRUC = v.Company?.RUC ?? "",
            DailyRate = v.Pricing?.DailyRate,
            WeeklyRate = v.Pricing?.WeeklyRate,
            Discount = v.Pricing?.Discount
        }).ToList();
    }

    /// <summary>
    /// Convierte DTOs de cache a entidades Vehicle con datos completos
    /// </summary>
    private IEnumerable<Vehicle> ConvertFromCacheDto(IEnumerable<VehicleCacheDto> dtos)
    {
        return dtos.Select(dto => 
        {
            // Crear la entidad Vehicle con el constructor
            var vehicle = new Vehicle(
                dto.Id,
                dto.Passengers,
                dto.LuggageCapacity,
                dto.LicensePlate,
                dto.Color,
                dto.Year,
                dto.Transmission,
                dto.FuelType,
                dto.ImageUrl ?? string.Empty,
                dto.ModelId,
                dto.BrandId,
                dto.CompanyId
            );

            // Restaurar las relaciones desde el DTO de cache
            // Crear entidades con datos completos para que las propiedades navigation funcionen
            if (!string.IsNullOrEmpty(dto.ModelName))
            {
                vehicle.Model = new VehicleService.VehicleBounded.Domain.Model.Entities.Model 
                { 
                    Id = dto.ModelId, 
                    CarModel = dto.ModelName,
                    BrandId = dto.BrandId
                };
            }

            if (!string.IsNullOrEmpty(dto.BrandName))
            {
                vehicle.Brand = new VehicleService.VehicleBounded.Domain.Model.Entities.Brand 
                { 
                    Id = dto.BrandId, 
                    BrandName = dto.BrandName 
                };
            }

            if (!string.IsNullOrEmpty(dto.CompanyName))
            {
                vehicle.Company = new VehicleService.VehicleBounded.Domain.Model.Entities.Company 
                { 
                    Id = dto.CompanyId, 
                    Name = dto.CompanyName,
                    RUC = dto.CompanyRUC ?? string.Empty
                };
            }

            if (dto.DailyRate.HasValue)
            {
                vehicle.Pricing = new VehicleService.VehicleBounded.Domain.Model.Entities.Pricing(
                    dto.DailyRate.Value,
                    dto.WeeklyRate ?? 0,
                    dto.Discount ?? 0,
                    dto.Id
                );
            }

            return vehicle;
        });
    }
}