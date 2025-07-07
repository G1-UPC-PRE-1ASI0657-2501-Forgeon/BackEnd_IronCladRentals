using Microsoft.EntityFrameworkCore;
using RentalService.RentalBounded.Domain.Model.Aggregates;

namespace RentalService.RentalBounded.Infraestructure.Persistence.EFC;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.StartDate)
                .IsRequired();

            entity.Property(r => r.EndDate)
                .IsRequired();

            entity.Property(r => r.RentalStatus)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(r => r.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(r => r.UserId)
                .IsRequired();

            entity.Property(r => r.VehicleId)
                .IsRequired();
            entity.Property(r => r.CompanyId)
                .IsRequired();
        });
    }
}