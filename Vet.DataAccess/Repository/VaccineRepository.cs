using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class VaccineRepository(AuthDbContext authDbContext) : Repository<Vaccine>(authDbContext), IVaccineRepository {
    public void Update(Vaccine vaccine)
    {
        authDbContext.Vaccines.Update(vaccine);
    }
}
