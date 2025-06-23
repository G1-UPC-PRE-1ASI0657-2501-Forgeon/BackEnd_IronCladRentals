namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record VehicleResource(
    int Passengers,
    int LuggageCapacity,
    string LicensePlate,
    string Color,
    int Year,
    string Transmission,
    string FuelType,
    string ImageUrl,
    string Address,
    string City,
    string Country,
    decimal Latitude,
    decimal Longitude,
    int ModelId,
    string ModelName,
    int BrandId,
    string BrandName,
    int CompanyId,
    string CompanyName,
    PricingResource Pricing
);