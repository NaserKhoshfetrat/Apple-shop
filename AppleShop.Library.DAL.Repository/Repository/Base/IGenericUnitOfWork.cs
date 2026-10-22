namespace AppleShop.Library.DAL.Repository.Repository.Base
{
    public interface IGenericUnitOfWork : IDisposable
    {
        //IApplicationRepository Products { get; }

        /// <summary>
        /// Returns number of affected rows
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
