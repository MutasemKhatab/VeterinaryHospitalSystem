using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;

namespace Vet.DataAccess.Repository;

public class UnitOfWork(AuthDbContext authDbContext, VeterinaryDbContext veterinaryDbContext) : IUnitOfWork
{
    public IVetOwnerRepository VetOwner { get; } = new VetOwnerRepository(authDbContext);
    public IVeterinarianRepository Veterinarian { get; } = new VeterinarianRepository(authDbContext);
    public IVetRepository Vet { get; } = new VetRepository(authDbContext);
    public IVaccineRepository Vaccine { get; } = new VaccineRepository(authDbContext);

    public async Task SaveAsync(string whichDb = "auth")
    {
        if (whichDb == "auth")
            await authDbContext.SaveChangesAsync();
        else
            await veterinaryDbContext.SaveChangesAsync();
    }
}
