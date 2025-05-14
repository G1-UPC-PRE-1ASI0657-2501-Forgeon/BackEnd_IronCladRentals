using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Repositories;

public interface IPricingRepository : IBaseRepository<Pricing>
{
    Task<Pricing?> GetByVehicleIdAsync(int vehicleId);
    Task<List<Pricing>> GetAllAsync();
    Task<Pricing?> GetByIdAsync(int pricingId);
}
