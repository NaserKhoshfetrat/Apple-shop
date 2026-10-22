using AppleShop.Services.ProductApi.Models.Dtos;

namespace AppleShop.Services.ProductApi.Services.ApiServices
{
    public interface IProductApiService
    {
        Task<BaseResponseDto<List<ProductDto>>> ReadProductPageAsync(int pageSize, int pageIndex = 0);
        Task<BaseResponseDto<ProductDto>> ReadProductDetailAsync(long pid);
        Task<BaseResponseDto<ProductDto>> CreateProductAsync(ProductDto productDto);
        Task<BaseResponseDto<ProductDto>> UpdateProductAsync(ProductDto productDto);
        Task<BaseResponseDto<ProductDto>> DeleteProductAsync(long productId);
    }
}