using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IVeterinarianRepository : IRepository<Veterinarian>
{
    void Update(Veterinarian veterinarian);
}