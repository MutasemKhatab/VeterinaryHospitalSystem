
namespace Vet.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IVetOwnerRepository VetOwner { get; }
    
    Task SaveAsync(string whichDb="auth");
}