using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IVehicleCommandService
{
    Task<Vehicle> CreateVehicleAsync(Vehicle vehicle);
    Task<Vehicle> UpdateVehicleAsync(int vehicleId, Vehicle updatedVehicle);
    Task<bool> DeleteVehicleAsync(int vehicleId);
}
