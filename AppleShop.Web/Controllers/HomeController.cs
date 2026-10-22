using AppleShop.Web.Models;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Product;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart;
using AppleShop.Web.Services.Integration.Product;
using AppleShop.Web.Services.Integration.ShoppingCart;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace AppleShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductServices productServices;
        private readonly IShoppingCartService shoppingCartService;
        public HomeController(ILogger<HomeController> logger, IProductServices productServices, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            ViewData["Title"] = "Home";
            this.productServices = productServices;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
            int pageSize = 1000, pageIndex = 0;
            var accessToken = "";

            BaseResponseDto<List<ProductDto>> response = new();
            response = await productServices.GetProductsAsync(pageSize, pageIndex, accessToken);

            if (response != null)
            {
                if (response.IsSuccessful)
                {
                    return View(response.Result);
                }
                else
                {
                    ViewBag.Errors = response.Error;
                    return View();
                }
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Details([FromQuery] long ProductId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            BaseResponseDto<ProductDto?> response = new();
            response = await productServices.GetProductDetailAsync(pid: ProductId, accessToken);

            // If the response itself is null or the API call failed
            if (response == null || !response.IsSuccessful)
            {
                // Redirect to Index with an error message
                TempData["Error"] = response.Error?.FirstOrDefault() ?? "An unknown error occurred.";
                //ViewBag.Errors = response.Error;
                return RedirectToAction("Index");

                // Return a NotFound (404) page
                // return NotFound();

                // Return a dedicated "Error" view
                // return View("Error", new ErrorViewModel { Message = response?.Error });
            }

            // If the result (ProductDto) itself is null
            if (response.Result == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            return View(response.Result);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost([FromForm] ProductDto productDto)
        {
            //if (!ModelState.IsValid)
            //    return View(productDto);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //var userId = User.FindFirst("sub")?.Value;
            var userId = User.Claims.First(u => u.Type == "sub")?.Value;

            var productDetailResponse = await productServices.GetProductDetailAsync(pid: productDto.ProductId, accessToken);

            if (productDetailResponse == null || !productDetailResponse.IsSuccessful)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            ShoppingCartDto shoppingCartDto = new()
            {
                ShoppingCartHeader = new ShoppingCartHeaderDto()
                {
                    //Id = 
                    UserId = userId!
                },
                ShoppingCartDetail = [new ShoppingCartDetailDto()
                {
                    Count = productDto.CountInShoppingCart,
                    ProductId = productDto.ProductId,
                    Product = productDetailResponse!.Result!,
                    CreatedAt = DateTime.UtcNow
                }]
            };
            
            BaseResponseDto<ShoppingCartDto> serviceOperationResult = await shoppingCartService.AddShoppingCartAsync(shoppingCartDto, accessToken);
            if (serviceOperationResult == null || !serviceOperationResult.IsSuccessful)
            {
                TempData["Error"] = serviceOperationResult?.Error?.FirstOrDefault() ?? "Failed to add to cart.";
                //return RedirectToAction(nameof(Index));
            }
            //else
            //{
            //    return View(productDto);
            //}

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

    }
}