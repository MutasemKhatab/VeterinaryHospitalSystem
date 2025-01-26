using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vet.Models;

namespace Vet.DataAccess.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<VetOwner>(options)
{
    public DbSet<VetOwner> VetOwners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<VetOwner>()
            .HasMany(e => e.Vets)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<VetOwner>(entity => { entity.ToTable("VetOwners"); });
        modelBuilder.Entity<Models.Vet>(entity => { entity.ToTable("Vets"); });
    }
}