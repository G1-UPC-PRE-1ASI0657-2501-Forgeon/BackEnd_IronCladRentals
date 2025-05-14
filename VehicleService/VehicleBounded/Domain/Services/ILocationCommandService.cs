using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface ILocationCommandService
{
    Task<Location> CreateLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(int locationId, Location updatedLocation);
    Task<bool> DeleteLocationAsync(int locationId);
}
