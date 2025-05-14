using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    // Métodos de IBaseRepository
    public async Task<Brand?> FindByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<IEnumerable<Brand>> ListAsync()
    {
        return await _context.Brands.ToListAsync();
    }

    public async Task AddSync(Brand entity)
    {
        await _context.Brands.AddAsync(entity);
    }

    public async Task AddAsync(Brand entity)
    {
        await _context.Brands.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand entity)
    {
        _context.Brands.Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Brand entity)
    {
        _context.Brands.Update(entity);
    }

    public void Remove(Brand entity)
    {
        _context.Brands.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand is not null)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }
    }

    // Métodos específicos de IBrandRepository
    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _context.Brands.ToListAsync();
    }
}