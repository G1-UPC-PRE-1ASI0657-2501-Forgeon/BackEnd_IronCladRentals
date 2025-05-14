using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class LocationTransform
{
    public static LocationResource ToResourceFromEntity(Location location) =>
        new(location.Id, location.Address, location.City, location.Country, location.LocationStatus, location.Latitude, location.Longitude);
}