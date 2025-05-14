using AuthService.User.Application.Internal.OutboundServices;
using AuthService.User.Domain.Model.Aggregates;
using AuthService.User.Domain.Model.Commands;
using AuthService.User.Domain.Repositories;
using AuthService.User.Domain.Services;
using IronClead.SharedKernel.Shared.Domain.Repositories;


namespace AuthService.User.Application.Internal.CommandServices;

public class AuthUserCommandService(
    IAuthUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IAuthUserCommandService
{
    /**
     * <summary>
     *     Handle sign in command
     * </summary>
     * <param name="command">The sign in command</param>
     * <returns>The authenticated user and the JWT token</returns>
     */
    public async Task<(AuthUser authUser, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByEmailAsync(command.Email);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");

        var token = tokenService.GenerateToken(user);

        return (user, token);
    }

   

    /**
     * <summary>
     *     Handle sign up command
     * </summary>
     * <param name="command">The sign up command</param>
     * <returns>A confirmation message on successful creation.</returns>
     */
    public async Task Handle(SignUpCommand command)
    {
        if (userRepository.ExistsByEmail(command.Email))
            throw new Exception($"Email {command.Email} is already taken");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new AuthUser(command.Email, hashedPassword,command.Name,command.DNI,command.Phone,command.DateCreatedAt,command.Role);
        try
        {
            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while creating user: {e.Message}");
        }
    }
}