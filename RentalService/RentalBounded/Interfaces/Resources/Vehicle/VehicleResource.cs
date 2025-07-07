namespace RentalService.RentalBounded.Interfaces.Resources.Vehicle;

public class VehicleResource
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string BrandName { get; set; } = default!;
    public string ModelName { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string LicensePlate { get; set; } = default!;
}