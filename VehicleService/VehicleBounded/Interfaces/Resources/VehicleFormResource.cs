namespace VehicleService.VehicleBounded.Interfaces.Resources;

public class VehicleFormResource
{
    public int Passengers { get; set; }
    public int LuggageCapacity { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;

    public int ModelId { get; set; }
    public int BrandId { get; set; }


    // Imagen vendrá por separado en IFormFile
    public PricingResource? Pricing { get; set; }
}
