namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record VehicleResource(
    int VehicleId,
    int Passengers,
    int LuggageCapacity,
    string LicensePlate,
    string Color,
    int Year,
    string Transmission,
    string FuelType,
    string ImageUrl,
    int ModelId,
    string ModelName,
    int BrandId,
    string BrandName,
    int CompanyId,
    string CompanyName,
    PricingResource Pricing
);