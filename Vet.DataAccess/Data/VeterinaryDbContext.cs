using Microsoft.EntityFrameworkCore;

namespace Vet.DataAccess.Data;

public class VeterinaryDbContext(DbContextOptions<VeterinaryDbContext> options) : DbContext(options) { }