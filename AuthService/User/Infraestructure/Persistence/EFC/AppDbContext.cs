using AuthService.User.Domain.Model.Aggregates;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AuthService.User.Infraestructure.Persistence.EFC;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<AuthUser> AuthUsers { get; set; } = null!;
    public DbSet<AuthUserRefreshToken> AuthUsersRefreshTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole()));
        builder.EnableSensitiveDataLogging();
        builder.AddCreatedUpdatedInterceptor(); // solo si aún usas esa librería
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Autenticación
        modelBuilder.Entity<AuthUser>(authUser =>
        {
            authUser.HasKey(u => u.Id);
            authUser.Property(u => u.Id).IsRequired();
            authUser.Property(u => u.Email).IsRequired().HasMaxLength(255);
            authUser.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_AuthUser_Email");
            authUser.Property(u => u.PasswordHash).IsRequired();
            
        });

        modelBuilder.Entity<AuthUserRefreshToken>(refreshToken =>
        {
            refreshToken.HasKey(rt => rt.Id);
            refreshToken.HasOne(rt => rt.AuthUser)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}