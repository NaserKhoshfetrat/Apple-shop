using AppleShop.Web.Models;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart;
using Microsoft.Extensions.Options;

namespace AppleShop.Web.Services.Integration.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ShoppingCartApiClient _apiClient;
        private readonly ServiceConfiguration _serviceConfiguration;

        public ShoppingCartService(ShoppingCartApiClient apiClient, IOptions<ServiceConfiguration> options)
        {
            _apiClient = apiClient;
            _serviceConfiguration = options.Value;
        }

        public async Task<BaseResponseDto<ShoppingCartDto>> AddShoppingCartAsync(ShoppingCartDto shoppingCartDto, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.AddCartRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ShoppingCartDto> response = await _apiClient.PostAsync<BaseResponseDto<ShoppingCartDto>, ShoppingCartDto>(shoppingCartDto, apiUrl, authorizationToken, headers, path, queryString);
 
            return response;
        }

        public async Task<BaseResponseDto<bool?>> ApplyCoupon(ShoppingCartDto shoppingCartDto, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.ApplyCouponRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<bool?> response = await _apiClient.PostAsync<BaseResponseDto<bool?>, ShoppingCartDto>(shoppingCartDto, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }

        public async Task<BaseResponseDto<bool?>> RemoveCoupon(string userId, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.RemoveCouponRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = $"/{userId}";

            BaseResponseDto<bool?> response = await _apiClient.GetAsync<BaseResponseDto<bool?>>(apiUrl, authorizationToken, queryString, headers, path);

            return response;
        }

        public async Task<BaseResponseDto<ShoppingCartDto>> GetShoppingCartByUserIdAsync(string userId, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.GetCartRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = $"/{userId}";

            BaseResponseDto<ShoppingCartDto> response = await _apiClient.GetAsync<BaseResponseDto<ShoppingCartDto>>(apiUrl, authorizationToken, queryString, headers, path);

            return response;
        }

        public async Task<BaseResponseDto<bool>> RemoveFromShoppingCartAsync(long shoppingCartDetailId, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.RemoveCartRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = $"/{shoppingCartDetailId}";
            object body = null; // new { shoppingCartDetailId = shoppingCartDetailId };
            BaseResponseDto<bool> response = await _apiClient.PostAsync<BaseResponseDto<bool>, object>(body, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }

        public async Task<BaseResponseDto<ShoppingCartDto>> UpdateShoppingCartAsync(ShoppingCartDto shoppingCartDto, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.UpdateCartRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ShoppingCartDto> response = await _apiClient.PostAsync<BaseResponseDto<ShoppingCartDto>, ShoppingCartDto>(shoppingCartDto, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }

        public async Task<BaseResponseDto<bool>> Checkout(ShoppingCartHeaderDto shoppingCartHeaderDto, string? accessToken = null)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ShoppingCartApi.BaseUrl), _serviceConfiguration.ShoppingCartApi.CheckoutRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<bool> response = await _apiClient.PostAsync<BaseResponseDto<bool>, ShoppingCartHeaderDto>(shoppingCartHeaderDto, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }
    }
}