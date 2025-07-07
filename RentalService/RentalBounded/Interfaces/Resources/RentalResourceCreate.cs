namespace RentalService.RentalBounded.Interfaces.Resources;

public record RentalResourceCreate(
    Guid UserId,
    int VehicleId,
    int LocationId,
    DateTime StartDate,
    DateTime EndDate
);
