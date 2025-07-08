using RentalService.RentalBounded.Domain.Model.Aggregates;

namespace RentalService.RentalBounded.Domain.Repositories;

public interface IRentalRepository
{
    Task<IEnumerable<Rental>> GetAllAsync();
    Task<Rental?> GetByIdAsync(Guid id);
    Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Rental>> GetByVehicleIdAsync(int vehicleId);
    Task<IEnumerable<Rental>> GetByCompanyIdAsync(int companyId);
    

    Task<Rental> AddAsync(Rental rental);
    Task<Rental> Update(Rental rental);
    Task Remove(Rental rental);
}