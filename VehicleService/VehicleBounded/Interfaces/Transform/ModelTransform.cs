using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class ModelTransform
{
    public static ModelResource ToResourceFromEntity(Model model) =>
        new(model.Id, model.CarModel);
}
