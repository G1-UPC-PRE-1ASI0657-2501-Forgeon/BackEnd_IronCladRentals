namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record AssignBrandToVehicleCommand(
    int VehicleId,
    int BrandId
);
