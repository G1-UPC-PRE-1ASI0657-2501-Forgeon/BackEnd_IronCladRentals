using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetByIdAsync(int id);
    Task<List<Company>> GetAllAsync();
    Task<Company?> GetByUserIdAsync(Guid authUserId);

}
