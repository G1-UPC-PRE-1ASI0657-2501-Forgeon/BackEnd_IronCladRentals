namespace AuthService.User.Interfaces.REST.Resources;

public record SignUpResource(string Email, string Password,string Name,string DNI,string Phone,DateTime dateCreated,bool Role);