﻿using System.Linq.Expressions;

namespace People.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(Guid id);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression);
    }
}
