using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IBrandQueryService
{
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task<Brand?> GetBrandByIdAsync(int id);
}
