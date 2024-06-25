using System.Linq.Expressions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace UKG_Integration_Application.Database.BaseRepo
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly MSSQLContext _mssqlContext;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(MSSQLContext mssqlContext)
        {
            _mssqlContext = mssqlContext;
            _dbSet = _mssqlContext.Set<T>();
        }

        // Methods start here: 
        public async Task<T> AddAsync(T entity)
        {
            await _mssqlContext.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
        {
            await _mssqlContext.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public void Delete(T entity)
        {
            _mssqlContext.Set<T>().Remove(entity);
        }

        public ICollection<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return FindByCondition(expression, null);
        }

        public ICollection<T> FindByCondition(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _mssqlContext.Set<T>().Where(expression).AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query.ToList();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _mssqlContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _mssqlContext.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            _mssqlContext.Set<T>().Update(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _mssqlContext.SaveChangesAsync();
        }

        public bool Any()
        {
            return _dbSet.Any();
        }
    }
}
