using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IVehicleQueryService
{
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateTime startDate, DateTime endDate, string? city = null);
    Task<IEnumerable<Vehicle>> GetVehiclesByCompanyIdAsync(int companyId);
    Task<Vehicle?> GetVehicleDetailsAsync(int vehicleId);
}
