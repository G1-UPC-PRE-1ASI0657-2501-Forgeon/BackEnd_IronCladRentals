using AuthService.User.Domain.Model.Commands;
using AuthService.User.Interfaces.REST.Resources;


namespace AuthService.User.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        bool role = resource.Role;


        return new SignUpCommand(resource.Email, resource.Password,resource.Name,resource.DNI,resource.Phone,DateTime.UtcNow,role);
    }
}