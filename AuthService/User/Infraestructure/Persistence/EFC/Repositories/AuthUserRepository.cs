using AuthService.User.Domain.Model.Aggregates;
using AuthService.User.Domain.Repositories;
using IronClead.SharedKernel.Shared.Infraestructure.Persistences.EFC.Repositories;
using Microsoft.EntityFrameworkCore;


namespace AuthService.User.Infraestructure.Persistence.EFC.Repositories;

public class AuthUserRepository : BaseRepository<AuthUser,AppDbContext>, IAuthUserRepository
{
    private readonly AppDbContext _context;

    public AuthUserRepository(AppDbContext context) : base(context)
    {
        _context = context; // Inicializa el contexto
    }

    /// <summary>
    /// Encuentra un usuario por su correo electrónico
    /// </summary>
    /// <param name="email">El correo electrónico del usuario</param>
    /// <returns>El usuario si es encontrado</returns>
    public async Task<AuthUser?> FindByEmailAsync(string email)
    {
        return await _context.Set<AuthUser>()
            .FirstOrDefaultAsync(user => user.Email.Equals(email));
    }

    // ✅ Implementación del nuevo método para buscar por Refresh Token
    public async Task<AuthUser?> FindByRefreshTokenAsync(string refreshToken)
    {
        return await _context.AuthUsers
            .Include(u => u.RefreshTokens) 
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken && rt.ExpiryDate > DateTime.UtcNow));
    }
    /// <summary>
    /// Encuentra un usuario por su ID
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario si es encontrado</returns>
    public async Task<AuthUser?> FindByIdAsync(Guid? authUserId)
    {
        return await _context.AuthUsers
            .FirstOrDefaultAsync(u => u.Id == authUserId); 
    }
    /// <summary>
    /// Verifica si un usuario existe por su correo electrónico
    /// </summary>
    /// <param name="email">El correo electrónico del usuario</param>
    /// <returns>True si existe, False en caso contrario</returns>
    public bool ExistsByEmail(string email)
    {
        return _context.Set<AuthUser>().Any(user => user.Email.Equals(email));
    }
}