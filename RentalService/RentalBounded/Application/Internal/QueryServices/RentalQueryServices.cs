using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Domain.Repositories;
using RentalService.RentalBounded.Domain.Services;

namespace RentalService.RentalBounded.Application.Internal.QueryServices;

public class RentalQueryService(IRentalRepository rentalRepository) : IRentalQueryService
{
    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await rentalRepository.GetAllAsync();
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await rentalRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId)
    {
        return await rentalRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Rental>> GetByVehicleIdAsync(int vehicleId)
    {
        return await rentalRepository.GetByVehicleIdAsync(vehicleId);
    }
    
    public async Task<IEnumerable<Rental>> GetByCompanyIdAsync(int companyId)
    {
        return await rentalRepository.GetByVehicleIdAsync(companyId);
    }
}