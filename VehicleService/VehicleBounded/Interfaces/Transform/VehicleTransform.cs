using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

public static class VehicleTransform
{
    public static VehicleResource ToResourceFromEntity(Vehicle vehicle) =>
        new(
            vehicle.Passengers,
            vehicle.LuggageCapacity,
            vehicle.LicensePlate,
            vehicle.Color,
            vehicle.Year,
            vehicle.Transmission,
            vehicle.FuelType,
            vehicle.ImageUrl,
            vehicle.Address,
            vehicle.City,
            vehicle.Country,
            vehicle.Latitude,
            vehicle.Longitude,
            vehicle.ModelId,
            vehicle.BrandId,
            vehicle.CompanyId,
            PricingTransform.ToResourceFromEntity(vehicle.Pricing)
        );

    public static Vehicle ToEntityFromResource(VehicleResource resource)
{
    var vehicle = new Vehicle(
        resource.Passengers,
        resource.LuggageCapacity,
        resource.LicensePlate,
        resource.Color,
        resource.Year,
        resource.Transmission,
        resource.FuelType,
        resource.ImageUrl,
        resource.Address,
        resource.City,
        resource.Country,
        resource.Latitude,
        resource.Longitude,
        resource.ModelId,
        resource.BrandId,
        resource.CompanyId // ← este campo también lo necesitas en tu resource si no está aún

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
