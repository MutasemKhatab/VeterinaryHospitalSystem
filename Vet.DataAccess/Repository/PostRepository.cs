using Vet.DataAccess.Data;
using Vet.DataAccess.Repository.IRepository;
using Vet.Models;

namespace Vet.DataAccess.Repository;

public class PostRepository(AuthDbContext db) : Repository<Post>(db), IPostRepository {
    public void Update(Post post) {
        db.Posts.Update(post);
    }
}