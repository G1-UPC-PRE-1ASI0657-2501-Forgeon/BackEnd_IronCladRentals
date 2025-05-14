using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface IBrandRepository : IBaseRepository<Brand>
{
    Task<Brand?> GetByIdAsync(int id);
    Task<List<Brand>> GetAllAsync();
}
