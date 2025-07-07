namespace RentalService.RentalBounded.Interfaces.Resources;

public class RentalWithVehicleResource
{
    public Guid RentalId { get; set; }
    public Guid AuthUserId { get; set; }
    public int VehicleId { get; set; }
    public int LocationId { get; set; }
    public string RentalStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public string BrandName  { get; set; }
    public string ModelName { get; set; }
    public string Color { get; set; }
    public string LicensePlate { get; set; }
    
}