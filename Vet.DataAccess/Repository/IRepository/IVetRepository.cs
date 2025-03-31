using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IVetRepository : IRepository<Models.Vet>
{
    void Update(Models.Vet vet);
}
