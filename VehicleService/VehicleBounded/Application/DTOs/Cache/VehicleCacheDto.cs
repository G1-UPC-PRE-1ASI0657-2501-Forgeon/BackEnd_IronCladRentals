namespace VehicleService.VehicleBounded.Application.DTOs.Cache;

/// <summary>
/// DTO para cachear vehículos sin referencias circulares
/// </summary>
public class VehicleCacheDto
{
    public int Id { get; set; }
    public int Passengers { get; set; }
    public int LuggageCapacity { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    
    // Información relacionada (sin referencias circulares)
    public int ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyRUC { get; set; } = string.Empty;
    
    // Pricing info (si existe)
    public decimal? DailyRate { get; set; }
    public decimal? WeeklyRate { get; set; }
    public decimal? Discount { get; set; }
    
    // Metadatos de cache
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;
}
