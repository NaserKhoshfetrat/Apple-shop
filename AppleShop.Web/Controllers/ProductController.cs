using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;
using AppleShop.Web.Services.Integration.Product;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppleShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> logger;
        private readonly IProductServices _productServices;

        public ProductController(ILogger<ProductController> logger, IProductServices productServices)
        {
            this.logger = logger;
            _productServices = productServices;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["Title"] = "Products";
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> ProductIndex(int pageSize = 10, int pageIndex = 0)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<List<ProductDto>> response = await _productServices.GetProductsAsync(pageSize, pageIndex, accessToken);
            if (response.IsSuccessful)
            {
                /*  if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }*/
                return View(response.Result);
            }
            else
            {
                ViewBag.Errors = response.Error;
                return View();

            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                BaseResponseDto<ProductDto?> response = await _productServices.CreateAsync(productDto, accessToken);
                if (response.IsSuccessful)
                {
                    return RedirectToAction(nameof(ProductIndex), new { pageIndex = 0, pageSize = 100 });
                }
                else
                {
                    // Add errors to ModelState
                    if (response.Error.Any())
                    {
                        foreach (var error in response.Error)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");

                    }

                    return View(productDto);
                }
            }
            else
            {
                // ModelState is invalid 
                return View(productDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(long pid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto?> response = await _productServices.GetProductDetailAsync(pid, accessToken);
            if (response.IsSuccessful)
            {
                return View(response.Result);
            }
            else
            {
                return NotFound();

            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                productDto.CreatedAt = DateTime.UtcNow;
                BaseResponseDto<ProductDto?> response = await _productServices.UpdateAsync(productDto, accessToken);
                if (response.IsSuccessful)
                {
                    return RedirectToAction(nameof(ProductIndex), new { pageIndex = 0, pageSize = 100 });
                }
                else
                {
                    // Add errors to ModelState
                    if (response.Error.Any())
                    {
                        foreach (var error in response.Error)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");

                    }

                    return View(productDto);
                }
            }
            else
            {
                // ModelState is invalid 
                return View(productDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProduct(long pid)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto?> responseGet = await _productServices.GetProductDetailAsync(pid, accessToken);
            if (responseGet.IsSuccessful)
            {
                BaseResponseDto<ProductDto?> responseDelete = await _productServices.DeleteAsync(pid, accessToken);
                if (responseDelete.IsSuccessful)
                {
                    return RedirectToAction(nameof(ProductIndex), new { pageIndex = 0, pageSize = 100 });
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();

            }
        }
    }
}