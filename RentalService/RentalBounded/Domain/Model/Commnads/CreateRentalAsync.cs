using RentalService.RentalBounded.Domain.Model.Aggregates;

namespace RentalService.RentalBounded.Domain.Model.Commnads;

public record CreateRentalAsync(Rental rental);