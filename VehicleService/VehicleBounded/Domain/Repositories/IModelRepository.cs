using IronClead.SharedKernel.Shared.Domain.Repositories;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface IModelRepository : IBaseRepository<Model.Entities.Model>
{
    Task<Model.Entities.Model?> GetByIdAsync(int id);
    Task<List<Model.Entities.Model>> GetAllAsync();
}
