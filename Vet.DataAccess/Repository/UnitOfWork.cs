using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;

namespace Vet.DataAccess.Repository;

public class UnitOfWork(AuthDbContext authDbContext) : IUnitOfWork {
    public IVetOwnerRepository VetOwner { get; } = new VetOwnerRepository(authDbContext);
    public IVeterinarianRepository Veterinarian { get; } = new VeterinarianRepository(authDbContext);
    public IVetRepository Vet { get; } = new VetRepository(authDbContext);
    public IVaccineRepository Vaccine { get; } = new VaccineRepository(authDbContext);
    public IServiceRequestRepository ServiceRequest { get; } = new ServiceRequestRepository(authDbContext);
    public IPostRepository Post { get; } = new PostRepository(authDbContext);
    public ICaseStudyRepository CaseStudy { get; } = new CaseStudyRepository(authDbContext);

    public async Task SaveAsync(string whichDb = "auth") {
        await authDbContext.SaveChangesAsync();
    }
}