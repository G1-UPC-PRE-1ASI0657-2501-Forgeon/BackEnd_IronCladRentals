namespace VehicleService.VehicleBounded.Domain.Services;

public interface IModelCommandService
{
    Task<Model.Entities.Model> CreateModelAsync(Model.Entities.Model model);
    Task<Model.Entities.Model> UpdateModelAsync(int modelId, Model.Entities.Model updatedModel);
    Task<bool> DeleteModelAsync(int modelId);
}
