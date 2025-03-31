using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class VetRepository(AuthDbContext authDbContext) : Repository<Models.Vet>(authDbContext), IVetRepository {
    public void Update(Models.Vet vet)
    {
        authDbContext.Vets.Update(vet);
    }
}
