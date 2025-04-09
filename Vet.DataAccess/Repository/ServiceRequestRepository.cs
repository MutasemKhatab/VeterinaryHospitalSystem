using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class ServiceRequestRepository(AuthDbContext db) : Repository<ServiceRequest>(db), IServiceRequestRepository {
    public void Update(ServiceRequest serviceRequest)
    {
        db.ServiceRequests.Update(serviceRequest);
    }
}
