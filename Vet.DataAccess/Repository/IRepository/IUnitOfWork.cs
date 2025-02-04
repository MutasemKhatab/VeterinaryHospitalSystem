
namespace Vet.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IVetOwnerRepository VetOwner { get; }
    IVeterinarianRepository Veterinarian { get; }
    
    Task SaveAsync(string whichDb="auth");
}