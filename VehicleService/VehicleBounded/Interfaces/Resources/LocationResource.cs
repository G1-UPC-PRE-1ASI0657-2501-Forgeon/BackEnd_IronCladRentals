namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record LocationResource(string Address, string City, string Country, string LocationStatus, decimal Latitude, decimal Longitude);
