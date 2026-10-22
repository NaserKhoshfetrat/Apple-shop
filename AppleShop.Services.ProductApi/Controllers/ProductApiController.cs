using AppleShop.Services.ProductApi.Models.Dtos;
using AppleShop.Services.ProductApi.Services.ApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppleShop.Services.ProductApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        public ILogger<ProductApiController> _logger { get; set; }
        private readonly IProductApiService _productApiService;

        public ProductApiController(ILogger<ProductApiController> logger, IProductApiService productApiService)
        {
            _logger = logger;
            _productApiService = productApiService;
        }

        //GET /api/Product/GetProducts/{pageSize}/{pageIndex}
        [HttpGet]
        [AllowAnonymous]
        [Route("{pageSize}/{pageIndex}")]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BaseResponseDto<List<ProductDto>>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductsAsync([FromRoute] int pageSize, [FromRoute] int pageIndex)
        {
            try
            {
                return Ok(await _productApiService.ReadProductPageAsync(pageSize, pageIndex));
            }
            catch (Exception)
            {
                return Problem(detail: System.Net.HttpStatusCode.InternalServerError.ToString(), statusCode: (int?)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        //GET /api/Product/GetProductDetail?pid=pid
        [HttpGet]
        [Authorize]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BaseResponseDto<ProductDto>), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)System.Net.HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProductDetailAsync([FromQuery] long pid)
        {
            try
            {
                return Ok(await _productApiService.ReadProductDetailAsync(pid));
            }
            catch (Exception)
            {
                return Problem(detail: System.Net.HttpStatusCode.InternalServerError.ToString(), statusCode: (int?)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        //Post /api/Product/Create
        [HttpPost]
        [Authorize(Roles = ProjectConfigurations.RoleAdmin)]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BaseResponseDto<ProductDto>), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)System.Net.HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] ProductDto request)
        {
            try
            {
                return Ok(await _productApiService.CreateProductAsync(request));
            }
            catch (Exception)
            {
                return Problem(detail: System.Net.HttpStatusCode.InternalServerError.ToString(), statusCode: (int?)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        //PATCH /api/Product/Update
        [HttpPatch]
        [Authorize(Roles = ProjectConfigurations.RoleAdmin)]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BaseResponseDto<ProductDto>), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)System.Net.HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] ProductDto request)
        {
            try
            {
                return Ok(await _productApiService.UpdateProductAsync(request));
            }
            catch (Exception)
            {
                return Problem(detail: System.Net.HttpStatusCode.InternalServerError.ToString(), statusCode: (int?)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        //DELETE /api/Product/Delete?pid=pid
        [HttpDelete]
        [Authorize(Roles = ProjectConfigurations.RoleAdmin)]
        [Consumes(System.Net.Mime.MediaTypeNames.Application.Json)]
        [Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(BaseResponseDto<ProductDto>), (int)System.Net.HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IActionResult), (int)System.Net.HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromQuery] long pid)
        {
            try
            {
                return Ok(await _productApiService.DeleteProductAsync(pid));
            }
            catch (Exception)
            {
                return Problem(detail: System.Net.HttpStatusCode.InternalServerError.ToString(), statusCode: (int?)System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}