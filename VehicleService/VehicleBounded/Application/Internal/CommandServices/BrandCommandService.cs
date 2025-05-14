using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;


public class BrandCommandService : IBrandCommandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BrandCommandService(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Brand> CreateBrandAsync(Brand brand)
    {
        await _brandRepository.AddAsync(brand);
        await _unitOfWork.CompleteAsync();
        return brand;
    }

    public async Task<Brand> UpdateBrandAsync(int brandId, Brand updatedBrand)
    {
        var existingBrand = await _brandRepository.GetByIdAsync(brandId);
        if (existingBrand is null)
            throw new KeyNotFoundException($"Brand with ID {brandId} not found.");

        existingBrand.UpdateName(updatedBrand.BrandName);
        _brandRepository.Update(existingBrand);
        await _unitOfWork.CompleteAsync();

        return existingBrand;
    }

    public async Task<bool> DeleteBrandAsync(int brandId)
    {
        var brand = await _brandRepository.GetByIdAsync(brandId);
        if (brand is null) return false;

        _brandRepository.Remove(brand);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}