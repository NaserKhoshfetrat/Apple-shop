using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart;

namespace AppleShop.Web.Services.Integration.ShoppingCart
{
    public interface IShoppingCartService
    {
        Task<BaseResponseDto<ShoppingCartDto>> AddShoppingCartAsync(ShoppingCartDto shoppingCartDto, string? accessToken = null);
        public Task<BaseResponseDto<ShoppingCartDto>> GetShoppingCartByUserIdAsync(string userId, string? accessToken = null);
        Task<BaseResponseDto<bool?>> RemoveFromShoppingCartAsync(long shoppingCartDetailId, string? accessToken = null);
        Task<BaseResponseDto<ShoppingCartDto>> UpdateShoppingCartAsync(ShoppingCartDto shoppingCartDto, string? accessToken = null);
        Task<BaseResponseDto<bool?>> ApplyCoupon(ShoppingCartDto shoppingCartDto, string? accessToken = null);
        Task<BaseResponseDto<bool?>> RemoveCoupon(string userId, string? accessToken = null);

        Task<BaseResponseDto<bool?>> Checkout(ShoppingCartHeaderDto shoppingCartHeaderDto, string? accessToken = null);
    }
}
