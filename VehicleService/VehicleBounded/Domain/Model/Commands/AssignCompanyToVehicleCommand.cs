namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record AssignCompanyToVehicleCommand(
    int VehicleId,
    int CompanyId
);
