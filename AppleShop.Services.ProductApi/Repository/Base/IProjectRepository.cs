using AppleShop.Library.DAL.Repository.Models.Entities;
using AppleShop.Library.DAL.Repository.Repository.Base;

namespace AppleShop.Services.ProductApi.Repository.Base
{
    public interface IProjectRepository<T, U> : IGenericRepository<T, U>
            where T : class, IEntityMarker
            where U : struct

    {
    }
}