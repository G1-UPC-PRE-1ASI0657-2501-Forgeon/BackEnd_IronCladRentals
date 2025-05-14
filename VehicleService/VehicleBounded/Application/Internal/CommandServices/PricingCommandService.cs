using IronClead.SharedKernel.Shared.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.CommandServices;

public class PricingCommandService : IPricingCommandService
{
    private readonly IPricingRepository _pricingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PricingCommandService(IPricingRepository pricingRepository, IUnitOfWork unitOfWork)
    {
        _pricingRepository = pricingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Pricing> SetPricingForVehicleAsync(int vehicleId, Pricing pricing)
    {
        pricing.AssignToVehicle(vehicleId);
        await _pricingRepository.AddAsync(pricing);
        await _unitOfWork.CompleteAsync();
        return pricing;
    }

    public async Task<Pricing> UpdatePricingAsync(int pricingId, Pricing updatedPricing)
    {
        var existing = await _pricingRepository.GetByIdAsync(pricingId);
        if (existing is null)
            throw new KeyNotFoundException($"Pricing with ID {pricingId} not found.");

        existing.UpdateRates(updatedPricing.DailyRate, updatedPricing.WeeklyRate, updatedPricing.Discount);
        _pricingRepository.Update(existing);
        await _unitOfWork.CompleteAsync();
        return existing;
    }

    public async Task<bool> DeletePricingAsync(int pricingId)
    {
        var pricing = await _pricingRepository.GetByIdAsync(pricingId);
        if (pricing is null) return false;

        _pricingRepository.Remove(pricing);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}