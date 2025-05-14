namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record UpdateVehicleCommand(
    int VehicleId,
    int Passengers,
    int LuggageCapacity
);
