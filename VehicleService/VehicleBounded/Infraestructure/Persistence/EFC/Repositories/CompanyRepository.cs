using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Entities;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    // Métodos de IBaseRepository
    public async Task<Company?> FindByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<IEnumerable<Company>> ListAsync()
    {
        return await _context.Companies.ToListAsync();
    }

    public async Task AddSync(Company entity)
    {
        await _context.Companies.AddAsync(entity);
    }

    public async Task AddAsync(Company entity)
    {
        await _context.Companies.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company entity)
    {
        _context.Companies.Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(Company entity)
    {
        _context.Companies.Update(entity);
    }

    public void Remove(Company entity)
    {
        _context.Companies.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company is not null)
        {
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
        }
    }

    // Métodos específicos de ICompanyRepository
    public async Task<Company?> GetByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies.ToListAsync();
    }
    
    public async Task<Company?> GetByUserIdAsync(Guid authUserId)
    {
        return await _context.Companies.FirstOrDefaultAsync(c => c.UserId == authUserId);
    }

}