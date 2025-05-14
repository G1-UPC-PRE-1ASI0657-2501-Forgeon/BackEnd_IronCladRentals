using AuthService.User.Domain.Model.Aggregates;

namespace AuthService.User.Domain.Repositories;

public interface IAuthUserRefreshTokenRepository
{
    Task<AuthUserRefreshToken?> GetByTokenAsync(string token);
    Task<AuthUserRefreshToken?> GetByUserIdAsync(Guid userId);
    Task AddAsync(AuthUserRefreshToken refreshToken);
    Task RevokeAsync(AuthUserRefreshToken refreshToken);
    Task UpdateAsync(AuthUserRefreshToken refreshToken);
}