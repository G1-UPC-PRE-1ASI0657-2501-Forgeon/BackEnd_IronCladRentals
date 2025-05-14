namespace VehicleService.VehicleBounded.Interfaces.Resources;

public record PricingResource(int Id, decimal DailyRate, decimal WeeklyRate, decimal Discount);
