using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices;
using AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart;
using AppleShop.Web.Services.Integration.Coupon;
using AppleShop.Web.Services.Integration.Product;
using AppleShop.Web.Services.Integration.ShoppingCart;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppleShop.Web.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ProductController> logger;
        private readonly IProductServices _productServices;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICouponServices _couponServices;

        public ShoppingCartController(ILogger<ProductController> logger, IProductServices productServices, IShoppingCartService shoppingCartService, ICouponServices couponServices)
        {
            this.logger = logger;
            _productServices = productServices;
            _shoppingCartService = shoppingCartService;
            _couponServices = couponServices;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["Title"] = "Products";
            base.OnActionExecuting(context);
        }

        public async Task<IActionResult> ShoppingCartIndex()
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;
            ShoppingCartDto? cartDto = await GetUserCartDto(accessToken, userId);

            return View(cartDto);
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(ShoppingCartDto shoppingCartDto)
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;
            if (shoppingCartDto.ShoppingCartDetail is null)
            {
                shoppingCartDto.ShoppingCartDetail = Enumerable.Empty<ShoppingCartDetailDto>();
            }

            BaseResponseDto<bool?> response = await _shoppingCartService.ApplyCoupon(shoppingCartDto, accessToken);
            bool isResponseSucessful = response is not null && response.IsSuccessful;
            if (!isResponseSucessful) { return View(); }
            else { return RedirectToAction(nameof(ShoppingCartIndex)); }
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(ShoppingCartDto shoppingCartDto)
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;

            BaseResponseDto<bool?> response = await _shoppingCartService.RemoveCoupon(userId, accessToken);
            bool isResponseSucessful = response is not null && response.IsSuccessful;
            if (!isResponseSucessful) { return View(); }
            else { return RedirectToAction(nameof(ShoppingCartIndex)); }
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;

            BaseResponseDto<bool?> response = await _shoppingCartService.RemoveFromShoppingCartAsync(cartDetailsId, accessToken);
            bool isResponseSucessful = response is not null && response.IsSuccessful;
            if (!isResponseSucessful) { return View(); }
            else { return RedirectToAction(nameof(ShoppingCartIndex)); }
        }

        private async Task<ShoppingCartDto?> GetUserCartDto(string accessToken, string userId)
        {

            BaseResponseDto<ShoppingCartDto> cartDto = await _shoppingCartService.GetShoppingCartByUserIdAsync(userId, accessToken);

            bool isResponseSucessful = cartDto is not null && cartDto.IsSuccessful;
            if (!isResponseSucessful) { return null; }

            var shoppingCartTotalPrice = 0m;

            if (!string.IsNullOrEmpty(cartDto?.Result?.ShoppingCartHeader.CouponCode))
            {
                var couponResponse = await _couponServices.GetCouponCodeDiscountAsync(cartDto.Result.ShoppingCartHeader.CouponCode, accessToken);
                if (couponResponse.IsSuccessful)
                {
                    cartDto.Result.ShoppingCartHeader.DiscountTotal = (decimal)((couponResponse.Result?.DiscountAmount) ?? 0d);
                }
            }

            foreach (var item in cartDto!.Result?.ShoppingCartDetail!)
            {
                shoppingCartTotalPrice += (item.Product.Price * item.Count);
            }

            cartDto.Result.ShoppingCartHeader.OrderTotal = shoppingCartTotalPrice - cartDto.Result.ShoppingCartHeader.DiscountTotal;
            if (cartDto.Result.ShoppingCartHeader.OrderTotal < 0) {
                cartDto.Result.ShoppingCartHeader.OrderTotal = 0;
            }
            return cartDto?.Result;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;
            ShoppingCartDto? cartDto = await GetUserCartDto(accessToken, userId);
            return View(cartDto);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _shoppingCartService.Checkout(cartDto.ShoppingCartHeader, accessToken);
                if (!response.IsSuccessful)
                {
                    bool isDisplayMessageFound = string.Equals("DisplayMessage", response.Error.FirstOrDefault(), StringComparison.InvariantCultureIgnoreCase);
                    TempData["Error"] = isDisplayMessageFound ? response.Error.ElementAtOrDefault(1) : "Error";
                    return RedirectToAction(nameof(Checkout));
                }
                //redirect to [Confirmation] Action
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception e)
            {
                return View(cartDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation()
        {
            string accessToken = (await HttpContext.GetTokenAsync("access_token"))!;
            string userId = User.Claims.First(u => u.Type == "sub")?.Value!;
            return View();
        }

    }
}