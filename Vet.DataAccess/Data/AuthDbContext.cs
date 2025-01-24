using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vet.Models;

namespace Vet.DataAccess.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<VetOwner>(options)
{
    public DbSet<VetOwner> VetOwners { get; set; }

}