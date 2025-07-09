using VehicleService.VehicleBounded.Application.DTOs.Cache;
using VehicleService.VehicleBounded.Interfaces.Resources;

namespace VehicleService.VehicleBounded.Interfaces.Transform;

public static class VehicleCacheTransform
{
    /// <summary>
    /// Convierte VehicleCacheDto directamente a VehicleResource
    /// </summary>
    public static VehicleResource ToResourceFromCacheDto(VehicleCacheDto dto) =>
        new(
            dto.Id,
            dto.Passengers,
            dto.LuggageCapacity,
            dto.LicensePlate,
            dto.Color,
            dto.Year,
            dto.Transmission,
            dto.FuelType,
            dto.ImageUrl ?? string.Empty,
            dto.ModelId,
            dto.ModelName,
            dto.BrandId,
            dto.BrandName,
            dto.CompanyId,
            dto.CompanyName,
            dto.DailyRate.HasValue ? new PricingResource(dto.DailyRate.Value, dto.WeeklyRate ?? 0, dto.Discount ?? 0) : null
        );

    /// <summary>
    /// Convierte múltiples VehicleCacheDto a VehicleResource
    /// </summary>
    public static IEnumerable<VehicleResource> ToResourceFromCacheDto(IEnumerable<VehicleCacheDto> dtos) =>
        dtos.Select(ToResourceFromCacheDto);
}
