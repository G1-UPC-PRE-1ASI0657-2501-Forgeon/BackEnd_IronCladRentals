namespace VehicleService.VehicleBounded.Domain.Services;

public interface IModelQueryService
{
    Task<IEnumerable<Model.Entities.Model>> GetAllModelsAsync();
    Task<Model.Entities.Model?> GetModelByIdAsync(int id);
}
