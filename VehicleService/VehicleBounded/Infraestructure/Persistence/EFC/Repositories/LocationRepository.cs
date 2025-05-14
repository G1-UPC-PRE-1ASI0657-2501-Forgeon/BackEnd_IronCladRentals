using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly AppDbContext _context;

    public LocationRepository(AppDbContext context)
    {
        _context = context;
    }

    // Métodos del IBaseRepository
    public async Task<Location?> FindByIdAsync(int id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<IEnumerable<Location>> ListAsync()
    {
        return await _context.Locations.ToListAsync();
    }

    public async Task AddSync(Location entity)
    {
        await _context.Locations.AddAsync(entity);
    }

    public async Task AddAsync(Location entity)
    {
        await _context.Locations.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Location entity)
    {
        _context.Locations.Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Location entity)
    {
        _context.Locations.Update(entity);
    }

    public void Remove(Location entity)
    {
        _context.Locations.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location is not null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }

    // Métodos personalizados del ILocationRepository
    public async Task<Location?> GetByIdAsync(int id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<List<Location>> GetAllAsync()
    {
        return await _context.Locations.ToListAsync();
    }
}