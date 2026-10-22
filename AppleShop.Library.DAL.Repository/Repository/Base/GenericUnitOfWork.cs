using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppleShop.Library.DAL.Repository.Repository.Base
{
    /// <summary>
    /// When using this unit of work you need to implement project specific repository:
    /// protected IApplicationRepository? _applicationRepository;
    /// public IApplicationRepository Products => _applicationRepository ??= new ApplicationRepository(_context);
    /// </summary>
    public abstract class GenericUnitOfWork : IGenericUnitOfWork
    {
        protected readonly DbContext _context;
        protected IDbContextTransaction? _currentTransaction;
        

        public GenericUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            _currentTransaction ??= await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();

                if (_currentTransaction != null)
                    await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
