using IronClead.SharedKernel.Shared.Domain.Repositories;
using IronClead.SharedKernel.Shared.Infraestructure.Cache.Helpers;
using IronClead.SharedKernel.Shared.Infraestructure.Interfaces;
using Microsoft.Extensions.Logging;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;

public class VehicleCommandService : IVehicleCommandService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<VehicleCommandService> _logger;

    public VehicleCommandService(
        IVehicleRepository vehicleRepository, 
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<VehicleCommandService> logger)
    {
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Vehicle> CreateVehicleAsync(Vehicle vehicle)
    {
        await _vehicleRepository.AddAsync(vehicle);
        await _unitOfWork.CompleteAsync();
        
        // Invalidar cache relacionado con vehículos
        await InvalidateVehicleCacheAsync(vehicle.CompanyId, vehicle.Id);
        
        return vehicle;
    }

    public async Task<Vehicle> UpdateVehicleAsync(int vehicleId, Vehicle updatedVehicle)
    {
        var existing = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (existing == null) throw new KeyNotFoundException("Vehicle not found");

        var oldCompanyId = existing.CompanyId;

        existing.UpdateVehicleDetails(
            updatedVehicle.Passengers,
            updatedVehicle.LuggageCapacity,
            updatedVehicle.LicensePlate,
            updatedVehicle.Color,
            updatedVehicle.Year,
            updatedVehicle.Transmission,
            updatedVehicle.FuelType,
            updatedVehicle.ImageUrl,
            updatedVehicle.ModelId,
            updatedVehicle.BrandId,
            updatedVehicle.CompanyId
        );

        _vehicleRepository.Update(existing);
        await _unitOfWork.CompleteAsync();
        
        // Invalidar cache para la empresa antigua y nueva (si cambió)
        await InvalidateVehicleCacheAsync(oldCompanyId, vehicleId);
        if (oldCompanyId != existing.CompanyId)
        {
            await InvalidateVehicleCacheAsync(existing.CompanyId, vehicleId);
        }
        
        return existing;
    }

    public async Task<bool> DeleteVehicleAsync(int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle == null) return false;

        var companyId = vehicle.CompanyId;
        
        _vehicleRepository.Remove(vehicle);
        await _unitOfWork.CompleteAsync();
        
        // Invalidar cache relacionado
        await InvalidateVehicleCacheAsync(companyId, vehicleId);
        
        return true;
    }

    /// <summary>
    /// Invalida el cache relacionado con vehículos
    /// </summary>
    private async Task InvalidateVehicleCacheAsync(int companyId, int vehicleId)
    {
        try
        {
            var tasks = new List<Task>();

            // Invalidar cache específico del vehículo
            var vehicleCacheKey = CacheKeyHelper.GenerateEntityKey<Vehicle>(vehicleId);
            tasks.Add(_cacheService.RemoveAsync(vehicleCacheKey));

            // Invalidar cache de vehículos por empresa
            var companyCacheKey = CacheKeyHelper.GenerateKey(new[] { "vehicles_by_company", companyId.ToString() });
            tasks.Add(_cacheService.RemoveAsync(companyCacheKey));

            // Invalidar cache de vehículos disponibles (patrón)
            var availableVehiclesPattern = "*available_vehicles*";
            tasks.Add(_cacheService.RemoveByPatternAsync(availableVehiclesPattern));

            // Invalidar todos los caches relacionados con vehículos
            var vehiclePattern = CacheKeyHelper.GenerateEntityPattern<Vehicle>();
            tasks.Add(_cacheService.RemoveByPatternAsync(vehiclePattern));

            await Task.WhenAll(tasks);
            
            _logger.LogInformation($"Cache invalidated for vehicle {vehicleId} and company {companyId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error invalidating cache for vehicle {vehicleId} and company {companyId}");
            // No re-lanzar la excepción para que la operación principal no falle
        }
    }
}
