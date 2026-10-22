using AppleShop.Library.DAL.Repository.Models.Entities;
using System.Linq.Expressions;

namespace AppleShop.Library.DAL.Repository.Repository.Base
{
    public interface IGenericRepository<T, U>
            where T : class, IEntityMarker
            where U : struct
    {
        Task<T?> FindByIdAsync(U id);
        Task<IEnumerable<T>> FindPageAsync(int pageSize, int pageIndex = 0);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);
        Task<T> CreateOrUpdateAsync(T entity);
        bool Update(T entity);
        Task<T?> DeleteAsync(U id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}