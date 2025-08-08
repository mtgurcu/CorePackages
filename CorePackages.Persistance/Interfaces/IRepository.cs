using System.Linq.Expressions;

namespace CorePackages.Persistance.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<int> SaveAsync();
        void Save();
        Task<List<T>> GetAll();
        Task<T?> GetByIdAsync(object id);
        Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter);
        Task<List<T>> GetListByFilterEagerList(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includeProperties);
        Task<T> CreateAsync(T entity);
        void Update(T entity);
        void UpdateEntity(T exists, T newEntity);
        void Remove(T entity);
        Task<T?> GetByFilterEager(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties);
        Task<T> AddAsync(T entity);
        Task<List<T>> AddRangeAsync(List<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
}
