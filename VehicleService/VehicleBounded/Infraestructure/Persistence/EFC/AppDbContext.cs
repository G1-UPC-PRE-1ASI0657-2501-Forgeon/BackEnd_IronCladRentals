using Microsoft.EntityFrameworkCore;
using VehicleService.VehicleBounded.Domain.Model.Aggregates;
using VehicleService.VehicleBounded.Domain.Model.Entities;

namespace VehicleService.VehicleBounded.Infraestructure.Persistence.EFC;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Model> Models { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Pricing> Pricings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // VEHICLE
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Passengers).IsRequired();
            entity.Property(v => v.LuggageCapacity).IsRequired();

            entity.HasOne(v => v.Brand)
                  .WithMany()
                  .HasForeignKey(v => v.BrandId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Model)
                  .WithMany()
                  .HasForeignKey(v => v.ModelId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Company)
                  .WithMany()
                  .HasForeignKey(v => v.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Pricing)
                  .WithOne()
                  .HasForeignKey<Pricing>(p => p.VehicleId);

            
        });

        // BRAND
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.BrandName).IsRequired().HasMaxLength(80);
        });

        // MODEL
        modelBuilder.Entity<Model>()
            .HasOne(m => m.Brand)
            .WithMany(b => b.Models)
            .HasForeignKey(m => m.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // COMPANY
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(40);
            entity.Property(c => c.RUC).IsRequired();
        });

        // LOCATION
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Address).IsRequired().HasMaxLength(255);
            entity.Property(l => l.City).IsRequired().HasMaxLength(100);
            entity.Property(l => l.Country).IsRequired().HasMaxLength(100);
            entity.Property(l => l.LocationStatus).IsRequired().HasMaxLength(50);
            entity.Property(l => l.Latitude).IsRequired();
            entity.Property(l => l.Longitude).IsRequired();
            
            entity.HasOne(l => l.Company)
                .WithMany(c => c.Locations)
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

       

        // PRICING
        modelBuilder.Entity<Pricing>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.DailyRate).IsRequired();
            entity.Property(p => p.WeeklyRate).IsRequired();
            entity.Property(p => p.Discount).IsRequired();
        });
    }
}
