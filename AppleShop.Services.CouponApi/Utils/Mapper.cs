using AppleShop.Services.CouponApi.Models.Dtos;
using AppleShop.Services.CouponApi.Models.Entities;

namespace AppleShop.Services.CouponApi.Utils
{
    public static class Mapper
    {
        public static CouponResponseDto Map(this CouponEntity? entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return new CouponResponseDto
            {
                CouponCode = entity.CouponCode,
                CouponId = entity.CouponId,
                DiscountAmount = entity.DiscountAmount,
                CreatedAt = entity.CreatedAt
            };
        }

        public static IEnumerable<CouponResponseDto> Map(this IEnumerable<CouponEntity> entities)
        {
            return entities?.Select(e => e.Map()) ?? Enumerable.Empty<CouponResponseDto>();
        }
    }
}
