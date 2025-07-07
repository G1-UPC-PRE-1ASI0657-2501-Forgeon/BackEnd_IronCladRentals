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
            rental.LocationId,
            rental.StartDate,
            rental.EndDate,
            rental.RentalStatus,
            rental.TotalPrice
        );
    
    

    public static Rental ToEntityFromResource(RentalResourceCreate resource) =>
        new(
            resource.UserId,
            resource.VehicleId,
            resource.LocationId,
            resource.StartDate,
            resource.EndDate,
            "Pending",
            0 // O lo que uses como valor inicial
        );
}
