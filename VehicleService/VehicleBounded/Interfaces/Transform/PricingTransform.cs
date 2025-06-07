using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class PricingTransform
{
    public static PricingResource ToResourceFromEntity(Pricing pricing) =>
        new(pricing.DailyRate, pricing.WeeklyRate, pricing.Discount);
}