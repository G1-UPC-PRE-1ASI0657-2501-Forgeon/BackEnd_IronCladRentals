using Microsoft.EntityFrameworkCore;
using RentalService.RentalBounded.Domain.Model.Aggregates;
using RentalService.RentalBounded.Domain.Repositories;

namespace RentalService.RentalBounded.Infraestructure.Persistence.EFC.Repositories;

public class RentalRepository(AppDbContext context) : IRentalRepository
{
    public async Task<Rental> AddAsync(Rental rental)
    {
        context.Rentals.Add(rental);
        await context.SaveChangesAsync();
        return rental;
    }

    public async Task<Rental?> GetByIdAsync(Guid id)
    {
        return await context.Rentals.FindAsync(id);
    }

    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        return await context.Rentals.ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByUserIdAsync(Guid userId)
    {
        return await context.Rentals
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetByVehicleIdAsync(int vehicleId)
    {
        return await context.Rentals
            .Where(r => r.VehicleId == vehicleId)
            .ToListAsync();
    }

    public async Task<Rental> Update(Rental rental)
    {
        context.Rentals.Update(rental);
        await context.SaveChangesAsync();
        return rental;
    }

    public async Task Remove(Rental rental)
    {
        context.Rentals.Remove(rental);
        await context.SaveChangesAsync();
    }
}
