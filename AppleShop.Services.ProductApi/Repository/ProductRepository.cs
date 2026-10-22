using AppleShop.Services.ProductApi.DbContexts;
using AppleShop.Services.ProductApi.Models.Entities;
using AppleShop.Services.ProductApi.Repository.Base;

namespace AppleShop.Services.ProductApi.Repository
{
    public interface IProductRepository : IProjectRepository<ProductEntity, long>
    {
        Task<List<(long ProductId, string? ImageUrl)>> GetImageUrl(long productId);
        bool UpdatePartial_ProductDescriptionAndPrice(ProductEntity dtoStub);
    }

    public class ProductRepository : ProjectRepository<ProductEntity, long>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<(long ProductId, string? ImageUrl)>> GetImageUrl(long productId)
        {
            var result = await base.FindAsync(
                p => p.ProductId == productId,
                p => new { p.ProductId, p.ImageUrl }
            );

            // Convert anonymous type to tuple
            return result.Select(x => (x.ProductId, x.ImageUrl)).ToList();
        }

        public bool UpdatePartial_ProductDescriptionAndPrice(ProductEntity dtoStub)
        {
            if (dtoStub.ProductId == 0)
                return false;

            var product = new ProductEntity
            {
                ProductId = dtoStub.ProductId,
                Description = dtoStub.Description,
                Price = dtoStub.Price
            };

            _dbContext.Products.Attach(product);

            _dbContext.Entry(product).Property(p => p.Description).IsModified = true;
            _dbContext.Entry(product).Property(p => p.Price).IsModified = true;

            return true;
        }
    }
}