using AppleShop.Library.DAL.Repository.Models.Entities;
using AppleShop.Library.DAL.Repository.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace AppleShop.Library.DAL.Repository.Repository.Base
{
    public class GenericRepository<T, U> : IGenericRepository<T, U>
        where T : class, IEntityMarker
        where U : struct
    {
        //protected readonly ApplicationDbContext _dbContext;
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        // Helper to get the primary key value (assumes property named "Id" or "{Some Entity Name}Id")
        private U GetIdValue(T entity)
        {
            PropertyInfo? prop = GetIdProperty();

            if (prop == null)
                throw new InvalidOperationException($"No Id property found on entity {typeof(T).Name}");

            return (U)prop.GetValue(entity);
        }

        private bool IsDefaultId(U id) => EqualityComparer<U>.Default.Equals(id, default(U));

        // Helper to get the primary key property name (used for ordering)
        private string GetIdPropertyName()
        {
            return GetIdProperty()?.Name ??
                   throw new InvalidOperationException("No Id property found");
        }

        private PropertyInfo? GetIdProperty()
        {
            return typeof(T).GetProperty("Id") ??
                   typeof(T).GetProperty($"{typeof(T).Name}Id") ??
                   typeof(T).GetProperty($"{typeof(T).Name.Replace("Entity", "")}Id") ??
                   throw new InvalidOperationException("No Id property found");
        }


        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<TResult>> FindAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return await _dbSet.Where(predicate).Select(selector).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindPageAsync(int pageSize, int pageIndex = 0)
        {
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be positive.");
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index must be non‑negative.");

            var query = _dbSet
                .OrderBy(e => EF.Property<U>(e, GetIdPropertyName())) //order by entity's id
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual async Task<T?> FindByIdAsync(U id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> CreateOrUpdateAsync(T entity)
        {
            U id = GetIdValue(entity);
            if (IsDefaultId(id))
            {
                await _dbSet.AddAsync(entity);
            }
            else
            {
                Update(entity);
            }
            return entity;
        }

        public virtual bool Update(T entity)
        {
            U id = GetIdValue(entity);
            if (IsDefaultId(id))
                return false;

            _dbSet.Update(entity);
            return true;
        }

        public virtual async Task<T?> DeleteAsync(U id)
        {
            var entity = await FindByIdAsync(id);
            if (entity is null)
                return null;

            _dbSet.Remove(entity);
            return entity;
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
