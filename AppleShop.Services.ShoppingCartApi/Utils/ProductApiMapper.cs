using AppleShop.Services.ShoppingCartApi.Models.Dtos;
using AppleShop.Services.ShoppingCartApi.Models.Entities;

namespace AppleShop.Services.ShoppingCartApi.Utils
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

        public static ShoppingCartHeaderDto Map(this ShoppingCartHeaderEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ShoppingCartHeaderDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CouponCode = entity.CouponCode
            };
        }

        public static ShoppingCartHeaderEntity Map(this ShoppingCartHeaderDto entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ShoppingCartHeaderEntity
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CouponCode = entity.CouponCode
            };
        }

        public static ShoppingCartDetailEntity Map(this ShoppingCartDetailDto input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return new ShoppingCartDetailEntity
            {
                ShoppingCartDetailId = input.ShoppingCartDetailId,
                ShoppingCartHeaderId = input.ShoppingCartHeaderId,
                ShoppingCartHeaderEntity = input.ShoppingCartHeader?.Map(),
                ProductId = input.ProductId,
                ProductEntity = input.Product?.Map(),
                Count = input.Count,
                CreatedAt = input.CreatedAt
            };
        }

        public static ShoppingCartDetailDto Map(this ShoppingCartDetailEntity input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            return new ShoppingCartDetailDto
            {
                ShoppingCartDetailId = input.ShoppingCartDetailId,
                ShoppingCartHeaderId = input.ShoppingCartHeaderId,
                ShoppingCartHeader = input.ShoppingCartHeaderEntity?.Map(), // uses your existing header mapper
                ProductId = input.ProductId,
                Product = input.ProductEntity?.Map(), // assumes a ProductEntity.Map() extension exists
                Count = input.Count,
                CreatedAt = input.CreatedAt
            };
        }

        public static ShoppingCartEntity Map(this ShoppingCartDto item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var result = new ShoppingCartEntity();
            result.ShoppingCartHeaderEntity = item.ShoppingCartHeader.Map();
            result.ShoppingCartDetailEntities = item.ShoppingCartDetail.Select(detail => detail.Map()).ToList() ?? Enumerable.Empty<ShoppingCartDetailEntity>();

            return result;
        }

        public static ShoppingCartDto Map(this ShoppingCartEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new ShoppingCartDto
            {
                ShoppingCartHeader = entity.ShoppingCartHeaderEntity?.Map(),
                ShoppingCartDetail = entity.ShoppingCartDetailEntities?
                    .Select(detail => detail.Map())
                    .ToList() ?? Enumerable.Empty<ShoppingCartDetailDto>()
            };
        }
    }
}