using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class BrandTransform
{
    public static BrandResource ToResourceFromEntity(Brand brand) =>
        new(brand.Id,brand.BrandName);
}