namespace RentalService.RentalBounded.Interfaces.Resources;

public record RentalResource(
    Guid Id,
    Guid UserId,
    int VehicleId,
    DateTime StartDate,
    DateTime EndDate,
    string RentalStatus,
    decimal TotalPrice
);
