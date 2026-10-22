using AppleShop.Web.Models;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Coupon;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;
using Microsoft.Extensions.Options;

namespace AppleShop.Web.Services.Integration.Coupon
{
    public class CouponServices : ICouponServices
    {
        private readonly CouponApiClient _apiClient;
        private readonly ServiceConfiguration _serviceConfiguration;

        public CouponServices(CouponApiClient apiClient, IOptions<ServiceConfiguration> options)
        {
            _apiClient = apiClient;
            _serviceConfiguration = options.Value;
        }

        public async Task<BaseResponseDto<CouponResponseDto>> GetCouponCodeDiscountAsync(string couponCode, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.CouponApi.BaseUrl), _serviceConfiguration.CouponApi.GetCouponRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = $"/{couponCode}";

            BaseResponseDto<CouponResponseDto> response = await _apiClient.GetAsync<BaseResponseDto<CouponResponseDto>>(apiUrl, authorizationToken, queryString, headers, path);

            return response;
        }
    }
}