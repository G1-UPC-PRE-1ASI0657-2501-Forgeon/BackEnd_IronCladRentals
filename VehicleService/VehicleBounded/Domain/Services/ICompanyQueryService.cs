using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Domain.Services;

public interface ICompanyQueryService
{
    /// <summary>
    /// Obtiene todas las empresas disponibles.
    /// </summary>
    Task<IEnumerable<Company>> GetAllAsync();

    /// <summary>
    /// Obtiene una empresa por su ID.
    /// </summary>
    Task<Company?> GetByIdAsync(int id);
    
    Task<Company?> GetByUserIdAsync(Guid userId);
}
