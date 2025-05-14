using System.Text.Json.Serialization;

namespace AuthService.User.Domain.Model.Aggregates;

public class AuthUser(string email, string passwordHash,string name,string dni,string phone,DateTime registeredAt,bool role)

{

    public AuthUser(): this(string.Empty, string.Empty,string.Empty,string.Empty,string.Empty,DateTime.UtcNow,true){}
    
    public Guid Id { get; }
    
    public string Email { get; private set; } = email;
    
    [JsonIgnore] public string PasswordHash { get; private set; } = passwordHash;
    public string Name { get; private set; } = name;
    public string DNI { get; private set; } = dni;
    public string Phone { get; set; } = phone; 
    
    public DateTime RegisteredAt { get; set; } = registeredAt;
    public bool Role { get; private set; } = role;
    
    public List<AuthUserRefreshToken> RefreshTokens { get; set; } = new();
    
    
    public AuthUser updateEmail(string email)
    {
        Email = email;
        return this;
    }

    public AuthUser updatePassword(string password)
    {
        PasswordHash = password;
        return this;
    }
    public void SetPassword(string newPassword)
    {
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    }


}