namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record LocationResource(int Id,string Address, string City, string Country, string LocationStatus, decimal Latitude, decimal Longitude,int companyId);
