using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;

namespace AppleShop.Web.Services.Integration.Product
{
    public interface IProductServices
    {
        Task<BaseResponseDto<ProductDto?>> CreateAsync(ProductDto dto, string accessToken);
        Task<BaseResponseDto<ProductDto?>> DeleteAsync(long pid, string accessToken);
        Task<BaseResponseDto<ProductDto?>> GetProductDetailAsync(long pid, string accessToken);
        Task<BaseResponseDto<List<ProductDto>>> GetProductsAsync(int pageSize, int pageIndex, string accessToken);
        Task<BaseResponseDto<ProductDto?>> UpdateAsync(ProductDto dto, string accessToken);
    }
}