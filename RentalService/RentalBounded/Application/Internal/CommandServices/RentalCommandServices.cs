using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Domain.Repositories;
using RentalService.RentalBounded.Domain.Services;

namespace RentalService.RentalBounded.Application.Internal.CommandServices;

public class RentalCommandService(IRentalRepository rentalRepository) : IRentalCommandService
{
    public async Task<Rental> CreateRentalAsync(Rental rental)
    {
        return await rentalRepository.AddAsync(rental);
    }

    public async Task<Rental> ExtendRentalAsync(Guid rentalId, DateTime newEndDate)
    {
        var rental = await rentalRepository.GetByIdAsync(rentalId);
        if (rental == null || rental.EndDate < DateTime.UtcNow) throw new InvalidOperationException("No se puede extender");

        rental.Extend(newEndDate);
        return await rentalRepository.Update(rental);
    }

    public async Task<bool> CancelRentalAsync(Guid rentalId)
    {
        var rental = await rentalRepository.GetByIdAsync(rentalId);
        if (rental == null) return false;

        rental.Cancel();
        await rentalRepository.Update(rental);
        return true;
    }

    public async Task<bool> CompleteRentalAsync(Guid rentalId)
    {
        var rental = await rentalRepository.GetByIdAsync(rentalId);
        if (rental == null) return false;

        rental.Complete();
        await rentalRepository.Update(rental);
        return true;
    }
    
    public async Task<bool> ConfirmRentalAsync(Guid rentalId)
    {
        var rental = await rentalRepository.GetByIdAsync(rentalId);
        if (rental == null) return false;

        rental.Confirm();
        await rentalRepository.Update(rental);
        return true;
    }


    public async Task<Rental> UpdateTotalPriceAsync(Guid rentalId, decimal newPrice)
    {
        var rental = await rentalRepository.GetByIdAsync(rentalId);
        if (rental == null) throw new KeyNotFoundException();

        rental.SetTotalPrice(newPrice);
        return await rentalRepository.Update(rental);
    }
}
