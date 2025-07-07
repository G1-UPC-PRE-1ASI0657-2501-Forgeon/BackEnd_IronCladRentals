using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class LocationQueryService : ILocationQueryService
{
    private readonly ILocationRepository _locationRepository;

    public LocationQueryService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        return await _locationRepository.GetAllAsync();
    }

    public async Task<Location?> GetLocationByIdAsync(int id)
    {
        return await _locationRepository.GetByIdAsync(id);
    }
    
    public async Task<IEnumerable<Location>> GetByCompanyIdAsync(int companyId)
    {
        return await _locationRepository.GetByCompanyIdAsync(companyId);
    }

    
}
