using AppleShop.Library.DAL.Repository.Models.Entities;
using AppleShop.Library.DAL.Repository.Repository.Base;
using AppleShop.Services.ProductApi.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.ProductApi.Repository.Base
{
    public class ProjectRepository<T, U> : GenericRepository<T, U>
        where T : class, IEntityMarker
        where U : struct
    {
        protected new readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public ProjectRepository(ApplicationDbContext dbContext)
            : base(dbContext)  // pass context to base
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }
    }
}
