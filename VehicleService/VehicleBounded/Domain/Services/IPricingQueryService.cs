using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IPricingQueryService
{
    Task<Pricing?> GetPricingByVehicleIdAsync(int vehicleId);
}
