using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface ILocationRepository : IBaseRepository<Location>
{
    Task<Location?> GetByIdAsync(int id);
    Task<List<Location>> GetAllAsync();
}
