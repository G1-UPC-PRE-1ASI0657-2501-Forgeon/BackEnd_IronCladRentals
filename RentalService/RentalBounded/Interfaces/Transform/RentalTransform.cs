using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Interfaces.Resources;

namespace RentalService.RentalBounded.Interfaces.Transform;

public static class RentalTransform
{
    public static RentalResource ToResourceFromEntity(Rental rental) =>
        new(
            rental.Id,
            rental.UserId,
            rental.VehicleId,
            rental.StartDate,
            rental.EndDate,
            rental.RentalStatus,
            rental.TotalPrice
        );

    public static Rental ToEntityFromResource(RentalResource resource) =>
        new(
            resource.UserId,
            resource.VehicleId,
            resource.StartDate,
            resource.EndDate,
            resource.RentalStatus,
            resource.TotalPrice
        )
        {
            // Si deseas mantener el mismo ID cuando actualices
            Id = resource.Id
        };
}
