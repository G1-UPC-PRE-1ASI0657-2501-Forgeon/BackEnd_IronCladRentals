using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class ModelRepository : IModelRepository
{
    private readonly AppDbContext _context;

    public ModelRepository(AppDbContext context)
    {
        _context = context;
    }

    // IBaseRepository implementation
    public async Task<Model?> FindByIdAsync(int id)
    {
        return await _context.Models.FindAsync(id);
    }

    public async Task<IEnumerable<Model>> ListAsync()
    {
        return await _context.Models.ToListAsync();
    }

    public async Task AddSync(Model entity)
    {
        await _context.Models.AddAsync(entity);
    }

    public async Task AddAsync(Model entity)
    {
        await _context.Models.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Model entity)
    {
        _context.Models.Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Model entity)
    {
        _context.Models.Update(entity);
    }

    public void Remove(Model entity)
    {
        _context.Models.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model is not null)
        {
            _context.Models.Remove(model);
            await _context.SaveChangesAsync();
        }
    }

    // Custom methods
    public async Task<List<Model>> GetAllAsync()
    {
        return await _context.Models.ToListAsync();
    }

    public async Task<Model?> GetByIdAsync(int id)
    {
        return await _context.Models.FindAsync(id);
    }
}