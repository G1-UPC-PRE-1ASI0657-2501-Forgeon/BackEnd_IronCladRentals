namespace RentalService.RentalBounded.Interfaces.Resources.Vehicle;

public class VehicleResource
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string BrandName { get; set; } = default!;
    public string ModelName { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string LicensePlate { get; set; } = default!;
    public PricingResource pricing { get; set; } = default!;
}

public class PricingResource
{
    public decimal DailyRate { get; set; }
    public decimal WeeklyRate { get; set; }
    public decimal Discount { get; set; }
}