using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface ILocationQueryService
{
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task<Location?> GetLocationByIdAsync(int id);
}
