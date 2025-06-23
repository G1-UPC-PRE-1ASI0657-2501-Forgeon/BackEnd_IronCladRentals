using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Repositories;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Vehicle?> FindByIdAsync(int id)
    {
        return await _context.Vehicles.FindAsync(id);
    }

    public async Task<IEnumerable<Vehicle>> ListAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }

 

    public async Task AddAsync(Vehicle entity)
    {
        await _context.Set<Vehicle>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    

    public void Update(Vehicle entity)
    {
        _context.Vehicles.Update(entity);
    }

    public void Remove(Vehicle entity)
    {
        _context.Vehicles.Remove(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        return await _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Model)
            .Include(v => v.Company)
            .Include(v => v.Pricing)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<List<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles
            .Include(v => v.Brand)
            .Include(v => v.Model)
            .Include(v => v.Company)
            .Include(v => v.Pricing)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetByBrandIdAsync(int brandId)
    {
        return await _context.Vehicles
            .Where(v => v.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetByModelIdAsync(int modelId)
    {
        return await _context.Vehicles
            .Where(v => v.ModelId == modelId)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetByCompanyIdAsync(int companyId)
    {
        return await _context.Vehicles
            .Where(v => v.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> SearchAvailableVehiclesAsync(DateTime startDate, DateTime endDate)
    {
        // Lógica de disponibilidad (placeholder: todos los vehículos están disponibles)
        return await _context.Vehicles
            .Include(v => v.Pricing)
            .Include(v => v.Company)
            .Include(v => v.Brand)
            .Include(v => v.Model)
            .ToListAsync();
    }

    

    public async Task DeleteAsync(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        await _context.SaveChangesAsync();
    }
}
