using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IVaccineRepository : IRepository<Vaccine>
{
    void Update(Vaccine vaccine);
}
