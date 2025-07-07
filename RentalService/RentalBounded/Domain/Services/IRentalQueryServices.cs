using RentalService.RentalBounded.Domain.Model.Aggregates;

namespace RentalService.RentalBounded.Domain.Services;

public interface IRentalQueryService
{
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(Guid rentalId);
    Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Rental>> GetByVehicleIdAsync(int vehicleId);
    Task<IEnumerable<Rental>> GetByCompanyIdAsync(int companyId);

}