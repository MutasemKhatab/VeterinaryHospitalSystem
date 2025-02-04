using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class VeterinarianRepository(AuthDbContext authDbContext) : Repository<Veterinarian>(authDbContext), IVeterinarianRepository {
    public void Update(Veterinarian veterinarian)
    {
        authDbContext.Veterinarians.Update(veterinarian);
    }
}