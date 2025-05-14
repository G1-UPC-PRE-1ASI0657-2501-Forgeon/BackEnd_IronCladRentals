using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class VehicleQueryService : IVehicleQueryService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleQueryService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate, string? city = null)
    {
        var availableVehicles = await _vehicleRepository.SearchAvailableVehiclesAsync(startDate, endDate);
        

        return availableVehicles;
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesByCompanyIdAsync(int companyId)
    {
        return await _vehicleRepository.GetByCompanyIdAsync(companyId);
    }

    public async Task<Vehicle?> GetVehicleDetailsAsync(int vehicleId)
    {
        return await _vehicleRepository.GetByIdAsync(vehicleId);
    }
}