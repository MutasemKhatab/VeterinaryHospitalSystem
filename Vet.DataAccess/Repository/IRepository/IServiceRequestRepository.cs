using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IServiceRequestRepository : IRepository<ServiceRequest>
{
    void Update(ServiceRequest serviceRequest);
}
