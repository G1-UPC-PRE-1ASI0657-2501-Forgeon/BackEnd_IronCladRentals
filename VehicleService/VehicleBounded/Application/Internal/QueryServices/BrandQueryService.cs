using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class BrandQueryService : IBrandQueryService
{
    private readonly IBrandRepository _brandRepository;

    public BrandQueryService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
    {
        return await _brandRepository.GetAllAsync();
    }

    public async Task<Brand?> GetBrandByIdAsync(int id)
    {
        return await _brandRepository.GetByIdAsync(id);
    }
}