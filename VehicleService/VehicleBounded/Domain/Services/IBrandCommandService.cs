using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface IBrandCommandService
{
    Task<Brand> CreateBrandAsync(Brand brand);
    Task<Brand> UpdateBrandAsync(int brandId, Brand updatedBrand);
    Task<bool> DeleteBrandAsync(int brandId);
}
