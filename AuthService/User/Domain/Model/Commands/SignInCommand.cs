namespace AuthService.User.Domain.Model.Commands;

public record SignInCommand(string Email, string Password);