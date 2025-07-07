using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class LocationTransform
{
    public static LocationResource ToResourceFromEntity(Location location) =>
    
        new(location.Id,location.Address, location.City, location.Country, location.LocationStatus, location.Latitude, location.Longitude,location.CompanyId);
    
    public static Location ToEntityFromResource(LocationResource resource)
    {
        return new Location(
            
            resource.Address,
            resource.City,
            resource.Country,
            resource.LocationStatus,
            resource.Latitude,
            resource.Longitude,
            resource.companyId
        );
    }

}