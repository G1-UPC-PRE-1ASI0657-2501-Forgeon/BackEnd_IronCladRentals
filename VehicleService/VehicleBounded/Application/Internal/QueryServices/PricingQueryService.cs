using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;
using VehicleService.VehicleBounded.Domain.Services;

namespace VehicleService.VehicleBounded.Application.Internal.QueryServices;

public class PricingQueryService : IPricingQueryService
{
    private readonly IPricingRepository _pricingRepository;

    public PricingQueryService(IPricingRepository pricingRepository)
    {
        _pricingRepository = pricingRepository;
    }

    public async Task<Pricing?> GetPricingByVehicleIdAsync(int vehicleId)
    {
        return await _pricingRepository.GetByVehicleIdAsync(vehicleId);
    }
}