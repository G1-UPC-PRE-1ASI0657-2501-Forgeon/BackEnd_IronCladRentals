namespace AuthService.User.Interfaces.REST.Resources;

public record AuthUserResource(Guid Id, string Email,string Name,string DNI,string Phone);