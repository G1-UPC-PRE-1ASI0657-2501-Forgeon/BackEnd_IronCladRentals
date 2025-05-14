namespace VehicleService.VehicleBounded.Domain.Model.Queries;

public record SearchAvailableVehiclesQuery(
    DateTime StartDate,
    DateTime EndDate,
    int? LocationId,
    int? MinPassengers,
    int? BrandId,
    string? VehicleType
);
