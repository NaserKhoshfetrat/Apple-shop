using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Coupon;

namespace AppleShop.Web.Services.Integration.Coupon
{
    public interface ICouponServices
    {
        Task<BaseResponseDto<CouponResponseDto>> GetCouponCodeDiscountAsync(string couponCode, string accessToken);
    }
}