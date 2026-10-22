using AppleShop.Library.DAL.Repository.Repository.Base;
using AppleShop.Services.ProductApi.DbContexts;

namespace AppleShop.Services.ProductApi.Repository.Base
{
    public class UnitOfWork : GenericUnitOfWork, IUnitOfWork
    {
        private new readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
            : base(context)  // pass context to base
        {
            this._context = context;
        }

        protected IProductRepository? _productRepository;

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
    }
}