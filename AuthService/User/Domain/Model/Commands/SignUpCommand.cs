namespace AuthService.User.Domain.Model.Commands;

public record SignUpCommand(string Email, string Password, string Name,string DNI, string Phone,DateTime DateCreatedAt, bool Role = true);
