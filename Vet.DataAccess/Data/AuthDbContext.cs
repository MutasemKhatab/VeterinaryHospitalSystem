using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vet.Models;

namespace Vet.DataAccess.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options) {
    public DbSet<VetOwner> VetOwners { get; set; }
    public DbSet<Veterinarian> Veterinarians { get; set; }
    public DbSet<Models.Vet> Vets { get; set; }
    public DbSet<Vaccine> Vaccines { get; set; }
    public DbSet<ServiceRequest> ServiceRequests { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        // VetOwner(one) -> Vet(many)
        modelBuilder.Entity<VetOwner>()
            .HasMany(e => e.Vets)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        // Vet(one) -> Vaccine(many)
        modelBuilder.Entity<Models.Vet>()
            .HasMany(e => e.Vaccines)
            .WithOne(e => e.Vet)
            .HasForeignKey(e => e.VetId)
            .OnDelete(DeleteBehavior.Cascade);
        // ServiceRequest(many) -> VetOwner
        modelBuilder.Entity<ServiceRequest>()
            .HasOne(e => e.VetOwner)
            .WithMany()
            .HasForeignKey(e => e.VetOwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        // Veterinarian(one) -> Post(many)
        modelBuilder.Entity<Veterinarian>()
            .HasMany<Post>()
            .WithOne(e => e.Veterinarian)
            .HasForeignKey(e => e.VeterinarianId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.Vet>(entity => { entity.ToTable("Vets"); });
        modelBuilder.Entity<Vaccine>(entity => { entity.ToTable("Vaccines"); });
        modelBuilder.Entity<ServiceRequest>(entity => { entity.ToTable("ServiceRequests"); });
        modelBuilder.Entity<Post>(entity => { entity.ToTable("Posts"); });
    }
}