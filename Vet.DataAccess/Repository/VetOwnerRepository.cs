using Microsoft.EntityFrameworkCore;
using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class VetOwnerRepository(AuthDbContext authDbContext)
    : Repository<VetOwner>(authDbContext),
        IVetOwnerRepository
{
    public void Update(VetOwner vetOwner)
    {
        authDbContext.VetOwners.Update(vetOwner);
    }

    public void UpdateVet(Vet.Models.Vet vet)
    {
        authDbContext.Entry(vet).State = EntityState.Modified;
    }

    public void DeleteVet(Vet.Models.Vet vet)
    {
        authDbContext.Entry(vet).State = EntityState.Deleted;
    }

}