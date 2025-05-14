namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record AddPricingToVehicleCommand(
    int VehicleId,
    decimal DailyRate,
    decimal WeeklyRate,
    decimal Discount
);
