using AppleShop.Services.ProductApi.Models.Dtos;
using AppleShop.Services.ProductApi.Repository.Base;
using AppleShop.Services.ProductApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.ProductApi.Services.ApiServices
{
    public class ProductApiService : IProductApiService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductApiService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseDto<ProductDto>> ReadProductDetailAsync(long pid)
        {
            try
            {
                var entity = await _unitOfWork.Products.FindByIdAsync(pid);

                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = (entity is not null),
                    Result = entity is not null ? entity.Map() : null,
                    Error = entity is not null ? [] : [System.Net.HttpStatusCode.NotFound.ToString()]
                };
            }
            catch (Exception)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.NotFound.ToString() },
                };
            }
        }

        public async Task<BaseResponseDto<List<ProductDto>>> ReadProductPageAsync(int pageSize, int pageIndex = 0)
        {
            try
            {
                var entity = await _unitOfWork.Products.FindPageAsync(pageSize, pageIndex);

                return new BaseResponseDto<List<ProductDto>>()
                {
                    IsSuccessful = entity.Any(),
                    Result = (entity.Any() ? entity.Select(x=> x.Map()) : Enumerable.Empty<ProductDto>()).ToList(),
                    Error = entity.Any() ? [] : [ System.Net.HttpStatusCode.NotFound.ToString() ]

                };
            }
            catch (Exception)
            {
                return new BaseResponseDto<List<ProductDto>>
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.NotFound.ToString() },
                };
            }
        }

        public async Task<BaseResponseDto<ProductDto>> CreateProductAsync(ProductDto productDto)
        {
            try
            {
                bool duplicatedName = await _unitOfWork.Products.AnyAsync(p => p.Name.ToLower() == productDto.Name.ToLower());
                if (duplicatedName)
                {
                    return new BaseResponseDto<ProductDto>()
                    {
                        IsSuccessful = false,
                        Result = null,
                        Error = { "A product with the same name already exists. Please use a different name." },
                    };
                }

                var entity = productDto.Map();

                await _unitOfWork.Products.CreateOrUpdateAsync(entity);
                _ = await _unitOfWork.SaveChangesAsync();

                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = (entity is not null),
                    Result = entity is not null ? entity.Map() : null,
                    Error = entity is not null ? [] : [System.Net.HttpStatusCode.NotFound.ToString()]
                };
            }
            catch (DbUpdateException)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.BadRequest.ToString() },
                };
            }
            catch (Exception)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.NotFound.ToString() },
                };
            }
        }

        public async Task<BaseResponseDto<ProductDto>> DeleteProductAsync(long productId)
        {
            try
            {
                var entity = await _unitOfWork.Products.DeleteAsync(productId);
                _ = await _unitOfWork.SaveChangesAsync();

                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = (entity is not null),
                    Result = entity is not null ? entity.Map() : null,
                    Error = entity is not null ? [] : [System.Net.HttpStatusCode.NotFound.ToString()]
                };
            }
            catch (Exception)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.NotFound.ToString() },
                };
            }
        }

        public async Task<BaseResponseDto<ProductDto>> UpdateProductAsync(ProductDto productDto)
        {
            try
            {
                bool duplicatedName = await _unitOfWork.Products.AnyAsync(p => p.Name.ToLower() == productDto.Name.ToLower());
                if (duplicatedName)
                {
                    return new BaseResponseDto<ProductDto>()
                    {
                        IsSuccessful = false,
                        Result = null,
                        Error = { "A product with the same name already exists. Please use a different name." },
                    };
                }

                var entity = productDto.Map();
                var isUpdateable = _unitOfWork.Products.Update(entity);
                _ = await _unitOfWork.SaveChangesAsync();

                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = isUpdateable,
                    Result = isUpdateable ? entity.Map() : null,
                    Error = entity is not null ? [] : [System.Net.HttpStatusCode.NotFound.ToString()]
                };
            }
            catch (DbUpdateException)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.BadRequest.ToString() },
                };
            }
            catch (Exception)
            {
                return new BaseResponseDto<ProductDto>()
                {
                    IsSuccessful = false,
                    Result = null,
                    Error = { System.Net.HttpStatusCode.NotFound.ToString() },
                };
            }
        }
    }
}