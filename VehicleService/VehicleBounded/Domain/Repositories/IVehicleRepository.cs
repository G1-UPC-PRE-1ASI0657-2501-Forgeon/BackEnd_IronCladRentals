using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface IVehicleRepository : IBaseRepository<Vehicle>
{
    Task<Vehicle?> GetByIdAsync(int id);
    Task<List<Vehicle>> GetAllAsync();
    Task<List<Vehicle>> GetByBrandIdAsync(int brandId);
    Task<List<Vehicle>> GetByModelIdAsync(int modelId);
    Task<List<Vehicle>> GetByCompanyIdAsync(int companyId);
    Task<List<Vehicle>> SearchAvailableVehiclesAsync(DateTime startDate, DateTime endDate);
}