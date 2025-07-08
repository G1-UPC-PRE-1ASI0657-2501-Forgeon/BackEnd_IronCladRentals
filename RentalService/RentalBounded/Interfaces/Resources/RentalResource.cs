using MySqlConnector.Logging;

namespace RentalService.RentalBounded.Interfaces.Resources;

public record RentalResource(
    Guid Id,
    Guid UserId,
    int VehicleId,
    int CompanyId,
    int LocationId,
    DateTime StartDate,
    DateTime EndDate,
    string RentalStatus,
    decimal TotalPrice,
    bool Paid
);
