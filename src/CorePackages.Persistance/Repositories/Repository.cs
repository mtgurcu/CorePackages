using CorePackages.Persistance.Entity;
using CorePackages.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CorePackages.Persistance.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<int> SaveAsync()
        {
            SetBaseFields();
            return _dbContext.SaveChangesAsync();
        }
        public void Save()
        {
            SetBaseFields();
            _dbContext.SaveChanges();
        }
        protected void SetBaseFields()
        {
            var entries = _dbContext.ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.UtcNow;
                        break;
                }
            }
        }
        public async Task<T> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);

            return entity;
        }
        public async Task<List<T>> GetAll()
        {
            List<T> allData;

            allData = await _dbContext.Set<T>().ToListAsync();

            return allData;
        }
        public async Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().Where(filter).ToListAsync();
        }

        public async Task<T?> GetByFilterEager(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync(filter);
        }
        public async Task<List<T>> GetListByFilterEagerList(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(filter).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public void Remove(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            _dbContext.Update(entity);
        }
        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }
        public void UpdateEntity(T exists, T newEntity)
        {
            _dbContext.Entry(exists).CurrentValues.SetValues(newEntity);
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            await _dbContext.AddRangeAsync(entities);
            return entities;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }
    }
}
