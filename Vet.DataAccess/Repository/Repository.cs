using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vet.DataAccess.Repository.IRepository;

namespace Vet.DataAccess.Repository;

public abstract class Repository<T>(DbContext db) : IRepository<T>
    where T : class {
    private readonly DbSet<T> _dbSet = db.Set<T>();

    public async Task AddAsync(T entity) {
        await _dbSet.AddAsync(entity);
    }

    public async Task<T?> Get(Expression<Func<T, bool>> filter, string? include = null, bool tracked = true) {
        var query = tracked ? _dbSet : _dbSet.AsNoTracking();

        query = query.Where(filter);
        Include(ref query, include);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? include = null) {
        IQueryable<T> query = _dbSet;
        if (filter != null)
            query = query.Where(filter);
        Include(ref query, include);

        return await query.ToListAsync();
    }

    public void Remove(T entity) {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity) {
        _dbSet.RemoveRange(entity);
    }

    private static void Include(ref IQueryable<T> query, string? include) {
        if (string.IsNullOrEmpty(include)) return;
        query = include.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Aggregate(query, (current, includeItem)
                => current.Include(includeItem));
    }
}
