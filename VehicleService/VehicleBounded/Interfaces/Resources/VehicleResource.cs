namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record VehicleResource(
    int Passengers,
    int LuggageCapacity,
    int ModelId,
    int BrandId,
    PricingResource Pricing
    );
