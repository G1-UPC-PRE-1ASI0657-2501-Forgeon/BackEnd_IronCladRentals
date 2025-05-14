using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IPricingCommandService
{
    Task<Pricing> SetPricingForVehicleAsync(int vehicleId, Pricing pricing);
    Task<Pricing> UpdatePricingAsync(int pricingId, Pricing updatedPricing);
    Task<bool> DeletePricingAsync(int pricingId);
}
