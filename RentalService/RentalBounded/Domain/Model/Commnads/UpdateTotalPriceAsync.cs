namespace RentalService.RentalBounded.Domain.Model.Commnads;

public record UpdateTotalPriceAsync(Guid rentalId,decimal newPrice);
