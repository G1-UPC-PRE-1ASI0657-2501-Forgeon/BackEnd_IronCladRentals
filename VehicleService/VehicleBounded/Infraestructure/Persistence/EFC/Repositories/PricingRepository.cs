using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class PricingRepository : IPricingRepository
{
    private readonly AppDbContext _context;

    public PricingRepository(AppDbContext context)
    {
        _context = context;
    }

    // IBaseRepository implementation
    public async Task<Pricing?> FindByIdAsync(int id)
    {
        return await _context.Pricings.FindAsync(id);
    }

    public async Task<IEnumerable<Pricing>> ListAsync()
    {
        return await _context.Pricings.ToListAsync();
    }

    public async Task AddSync(Pricing entity)
    {
        await _context.Pricings.AddAsync(entity);
    }

    public async Task AddAsync(Pricing entity)
    {
        await _context.Pricings.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Pricing entity)
    {
        _context.Pricings.Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Pricing entity)
    {
        _context.Pricings.Update(entity);
    }

    public void Remove(Pricing entity)
    {
        _context.Pricings.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var pricing = await _context.Pricings.FindAsync(id);
        if (pricing is not null)
        {
            _context.Pricings.Remove(pricing);
            await _context.SaveChangesAsync();
        }
    }

    // Custom methods
    public async Task<Pricing?> GetByVehicleIdAsync(int vehicleId)
    {
        return await _context.Pricings
            .FirstOrDefaultAsync(p => p.VehicleId == vehicleId);
    }

    public async Task<List<Pricing>> GetAllAsync()
    {
        return await _context.Pricings.ToListAsync();
    }

    public async Task<Pricing?> GetByIdAsync(int pricingId)
    {
        return await _context.Pricings.FindAsync(pricingId);
    }
}