namespace VehicleService.VehicleBounded.Domain.Model.Commands;

public record CreateVehicleCommand(
    int BrandId,
    int ModelId,
    int CompanyId,
    int Passengers,
    int LuggageCapacity
);