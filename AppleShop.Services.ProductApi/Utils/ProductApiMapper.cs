using AppleShop.Services.ProductApi.Models.Dtos;
using AppleShop.Services.ProductApi.Models.Entities;

namespace AppleShop.Services.ProductApi.Utils
{
    public static class ProductApiMapper
    {
        public static ProductDto Map(this ProductEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ProductDto
            {
                ProductId = entity.ProductId,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
                Price = entity.Price,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static IEnumerable<ProductDto> Map(this IEnumerable<ProductEntity> entities)
        {
            return entities?.Select(e => e.Map()) ?? Enumerable.Empty<ProductDto>();
        }

        public static ProductEntity Map(this ProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            return new ProductEntity
            {
                ProductId = dto.ProductId,
                CreatedAt = dto.CreatedAt,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };
        }

        public static IEnumerable<ProductEntity> Map(this IEnumerable<ProductDto> dtos)
        {
            return dtos?.Select(e => e.Map()) ?? Enumerable.Empty<ProductEntity>();
        }
    }
}
