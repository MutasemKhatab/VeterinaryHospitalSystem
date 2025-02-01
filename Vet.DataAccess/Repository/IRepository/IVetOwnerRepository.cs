using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IVetOwnerRepository :IRepository<VetOwner>
{
    void Update(VetOwner vetOwner);
    public void UpdateVet(Vet.Models.Vet vet);
    public void DeleteVet(Vet.Models.Vet vet);

}