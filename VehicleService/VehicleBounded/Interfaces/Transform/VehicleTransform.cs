using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Interfaces.Resources;
using VehicleService.VehicleBounded.Interfaces.Transform;

public static class VehicleTransform
{
    public static VehicleResource ToResourceFromEntity(Vehicle vehicle) =>
        new(
            vehicle.Id,
            vehicle.Passengers,
            vehicle.LuggageCapacity,
            vehicle.LicensePlate,
            vehicle.Color,
            vehicle.Year,
            vehicle.Transmission,
            vehicle.FuelType,
            vehicle.ImageUrl,
            vehicle.ModelId,
            vehicle.Model?.CarModel,
            vehicle.BrandId,
            vehicle.Brand?.BrandName,
            vehicle.CompanyId,
            vehicle.Company?.Name,
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
