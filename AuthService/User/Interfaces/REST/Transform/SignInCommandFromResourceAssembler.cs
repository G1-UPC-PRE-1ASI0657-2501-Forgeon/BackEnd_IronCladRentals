using AuthService.User.Domain.Model.Commands;
using AuthService.User.Interfaces.REST.Resources;


namespace AuthService.User.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email, resource.Password);
    }
}