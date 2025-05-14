using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class VehicleTransform
{
    public static VehicleResource ToResourceFromEntity(Vehicle vehicle) =>
        new(
            vehicle.Passengers,
            vehicle.LuggageCapacity,
            vehicle.ModelId,
            vehicle.BrandId,
            PricingTransform.ToResourceFromEntity(vehicle.Pricing)
        );
    public static Vehicle ToEntityFromResource(VehicleResource resource)
    {
        var vehicle = new Vehicle(
            resource.Passengers,
            resource.LuggageCapacity,
            resource.ModelId,
            resource.BrandId
            );

        if (resource.Pricing != null)
        {
            vehicle.SetPricing(
                resource.Pricing.DailyRate,
                resource.Pricing.WeeklyRate,
                resource.Pricing.Discount
            );
        }

       

        return vehicle;
    }

}
