using Vet.Models;

namespace Vet.DataAccess.Repository.IRepository;

public interface IPostRepository : IRepository<Post>
{
    void Update(Post post);
}
