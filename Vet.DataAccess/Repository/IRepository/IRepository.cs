﻿using System.Linq.Expressions;

namespace Vet.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<T?> Get(Expression<Func<T, bool>> filter, string? include = null, bool tracked = true);
    Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? include = null);
    Task AddAsync(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entity);
}
