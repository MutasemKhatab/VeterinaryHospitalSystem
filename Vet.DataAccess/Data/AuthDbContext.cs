using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vet.Models;

namespace Vet.DataAccess.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<VetOwner> VetOwners { get; set; }
    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<Models.Vet> Vets { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VetOwner>()
            .HasMany(e => e.Vets)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Vet>()
            .HasMany(e => e.Vaccines)
            .WithOne(e => e.Vet)
            .HasForeignKey(e => e.VetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Vet>(entity => { entity.ToTable("Vets"); });
        modelBuilder.Entity<Vaccine>(entity => { entity.ToTable("Vaccines"); });
    }
}