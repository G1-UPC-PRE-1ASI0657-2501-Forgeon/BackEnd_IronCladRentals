using RentalService.RentalBounded.Domain.Model.Aggregates;

namespace RentalService.RentalBounded.Domain.Services;

public interface IRentalCommandService
{
    Task<Rental> CreateRentalAsync(Rental rental);
    Task<Rental> ExtendRentalAsync(Guid rentalId, DateTime newEndDate);
    Task<bool> CancelRentalAsync(Guid rentalId);
    Task<bool> CompleteRentalAsync(Guid rentalId);
    Task<bool> ConfirmRentalAsync(Guid rentalId);
    Task<bool> PaidRentalAsync(Guid rentalId);
    Task<Rental> UpdateTotalPriceAsync(Guid rentalId, decimal newPrice);
}