using AppleShop.Web.Models;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;
using Microsoft.Extensions.Options;

namespace AppleShop.Web.Services.Integration.Product
{
    public class ProductServices : IProductServices
    {
        private readonly ProductApiClient _apiClient;
        private readonly ServiceConfiguration _serviceConfiguration;

        public ProductServices(ProductApiClient apiClient, IOptions<ServiceConfiguration> options)
        {
            _apiClient = apiClient;
            _serviceConfiguration = options.Value;
        }

        public async Task<BaseResponseDto<List<ProductDto>>> GetProductsAsync(int pageSize, int pageIndex, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ProductApi.BaseUrl), _serviceConfiguration.ProductApi.GetProductsRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = $"/{pageSize}/{pageIndex}";

            BaseResponseDto<List<ProductDto>> response = await _apiClient.GetAsync<BaseResponseDto<List<ProductDto>>>(apiUrl, authorizationToken, queryString, headers, path);

            return response;
        }

        public async Task<BaseResponseDto<ProductDto?>> CreateAsync(ProductDto dto, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ProductApi.BaseUrl), _serviceConfiguration.ProductApi.CreateRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ProductDto?> response = await _apiClient.PostAsync<BaseResponseDto<ProductDto?>, ProductDto>(dto, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }

        public async Task<BaseResponseDto<ProductDto?>> DeleteAsync(long pid, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ProductApi.BaseUrl), _serviceConfiguration.ProductApi.DeleteRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = new Dictionary<string, string> { { "pid", pid.ToString() } };
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ProductDto?> response = await _apiClient.DeleteAsync<BaseResponseDto<ProductDto?>, object>(null, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }

        public async Task<BaseResponseDto<ProductDto?>> GetProductDetailAsync(long pid, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ProductApi.BaseUrl), _serviceConfiguration.ProductApi.GetProductDetailRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = new Dictionary<string, string> { { "pid", pid.ToString() } };
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ProductDto?> response = await _apiClient.GetAsync<BaseResponseDto<ProductDto?>>(apiUrl, authorizationToken, queryString, headers, path);

            return response;
        }

        public async Task<BaseResponseDto<ProductDto?>> UpdateAsync(ProductDto dto, string accessToken)
        {
            string apiUrl = new Uri(new Uri(_serviceConfiguration.ProductApi.BaseUrl), _serviceConfiguration.ProductApi.UpdateRelativeUrl).ToString();
            string authorizationToken = accessToken;
            Dictionary<string, string>? queryString = null;
            Dictionary<string, string>? headers = null;
            string? path = null;

            BaseResponseDto<ProductDto?> response = await _apiClient.PatchAsync<BaseResponseDto<ProductDto?>, ProductDto>(dto, apiUrl, authorizationToken, headers, path, queryString);

            return response;
        }
    }
}