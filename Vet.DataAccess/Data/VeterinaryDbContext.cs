using Microsoft.EntityFrameworkCore;
using Vet.Models;

namespace Vet.DataAccess.Data {
    public class VeterinaryDbContext(DbContextOptions<VeterinaryDbContext> options) : DbContext(options);
}
