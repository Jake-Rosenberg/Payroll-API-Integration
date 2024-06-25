using System.Linq.Expressions;

namespace UKG_Integration_Application.Database.BaseRepo
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        ICollection<T> FindByCondition(Expression<Func<T, bool>> expression);
        ICollection<T> FindByCondition(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
        Task SaveChangesAsync();
        bool Any();
    }
}
