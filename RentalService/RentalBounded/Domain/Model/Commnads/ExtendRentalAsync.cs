namespace RentalService.RentalBounded.Domain.Model.Commnads;

public record ExtendRentalAsync(Guid rentalId,DateTime newEndDate);
