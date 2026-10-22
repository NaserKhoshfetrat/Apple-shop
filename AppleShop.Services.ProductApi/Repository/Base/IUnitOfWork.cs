using AppleShop.Library.DAL.Repository.Repository.Base;

namespace AppleShop.Services.ProductApi.Repository.Base
{
    public interface IUnitOfWork : IGenericUnitOfWork
    {
        IProductRepository Products { get; }
    }
}
