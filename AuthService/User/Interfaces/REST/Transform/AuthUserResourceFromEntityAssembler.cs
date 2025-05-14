using AuthService.User.Interfaces.REST.Resources;

namespace AuthService.User.Interfaces.REST.Transform;

public static class AuthUserResourceFromEntityAssembler
{
    public static AuthUserResource ToResourceFromEntity(Domain.Model.Aggregates.AuthUser user)
    {
        return new AuthUserResource(user.Id, user.Email,user.Name,user.DNI,user.Phone);
    }
}