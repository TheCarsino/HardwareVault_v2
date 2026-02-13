// ============================================================
// DOMAIN LAYER — INTERFACES
// ============================================================
// Generic base contract. Every entity-specific repository
// inherits this and adds its own specialized methods on top.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);
    }
}
