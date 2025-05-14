namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record AssignModelToVehicleCommand(
    int VehicleId,
    int ModelId
);
