
namespace Vet.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IVetOwnerRepository VetOwner { get; }
    IVeterinarianRepository Veterinarian { get; }
    IVetRepository Vet { get; }
    IVaccineRepository Vaccine { get; }
    IServiceRequestRepository ServiceRequest { get; }
    
    Task SaveAsync(string whichDb="auth");
}
