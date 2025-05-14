namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record UpdateVehiclePricingCommand(
    int VehicleId,
    decimal NewDailyRate,
    decimal NewWeeklyRate,
    decimal NewDiscount
);
