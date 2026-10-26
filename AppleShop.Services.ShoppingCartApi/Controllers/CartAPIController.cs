using AppleShop.Services.ShoppingCartApi.Models.Dtos;
using AppleShop.Services.ShoppingCartApi.Models.Message;
using AppleShop.Services.ShoppingCartApi.Repository;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AppleShop.Services.ShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartAPIController : Controller
    {
        private readonly IShoppingCartRepository _cartRepository;
        //protected ResponseDto _response;

        //private readonly ICouponRepository _couponRepository;
        //private readonly IMessageBus _messageBus;
        //private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;

        public CartAPIController(IShoppingCartRepository cartRepository//, IMessageBus messageBus, ICouponRepository couponRepository, IRabbitMQCartMessageSender rabbitMQCartMessageSender
            )
        {
            _cartRepository = cartRepository;
            //_couponRepository = couponRepository;
            //_rabbitMQCartMessageSender = rabbitMQCartMessageSender;
            //_messageBus = messageBus;
            //this._response = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<BaseResponseDto<ShoppingCartDto>> GetCart(string userId)
        {
            BaseResponseDto<ShoppingCartDto> response = new();
            
            try
            {
                ShoppingCartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                response.Result = cartDto;
            }
            catch (Exception ex)
            {
                response.IsSuccessful= false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpPost("AddCart")]
        public async Task<BaseResponseDto<ShoppingCartDto>> AddCart(ShoppingCartDto cartDto)
        {
            BaseResponseDto<ShoppingCartDto> response = new();

            try
            {
                ShoppingCartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                response.Result = cartDt;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpPost("UpdateCart")]
        public async Task<BaseResponseDto<ShoppingCartDto>> UpdateCart(ShoppingCartDto cartDto)
        {
            BaseResponseDto<ShoppingCartDto> response = new();

            try
            {
                ShoppingCartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                response.Result = cartDt;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpPost("RemoveCart/{shoppingCartDetailId}")]
        public async Task<BaseResponseDto<bool>> RemoveCart(int shoppingCartDetailId)
        {
            BaseResponseDto<bool> response = new();

            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(shoppingCartDetailId);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<BaseResponseDto<bool>> ApplyCoupon([FromBody] ShoppingCartDto shoppingCartDto)
        {
            BaseResponseDto<bool> response = new();
          
            if (!ModelState.IsValid)
            {
                //    return BadRequest(ModelState);
                response.IsSuccessful = false;
                response.Error = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return response;
            }

            try
            {
                bool isSuccess = await _cartRepository.ApplyCoupon(shoppingCartDto.ShoppingCartHeader.UserId, shoppingCartDto.ShoppingCartHeader.CouponCode);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpGet("RemoveCoupon/{userId}")]
        public async Task<BaseResponseDto<bool>> RemoveCoupon(string userId)
        {
            BaseResponseDto<bool> response = new();

            try
            {
                bool isSuccess = await _cartRepository.RemoveCoupon(userId);
                response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }

        [HttpPost("Checkout")]
        public async Task<BaseResponseDto<bool>> Checkout(CheckoutHeaderDto checkoutHeader)
        {
            BaseResponseDto<bool> response = new();
          
            try
            {
                ShoppingCartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
                if (cartDto == null)
                {
                    response.IsSuccessful = false;
                    response.Result = false;
                    response.Error = new List<string>() { HttpStatusCode.BadRequest.ToString() };
                    return response;
                }

                //if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
                //{
                    //ShoppingCartDto coupon = await _cartRepository.GetCoupon(checkoutHeader.CouponCode);
                //    if (checkoutHeader.DiscountTotal != coupon.DiscountAmount)
                //    {
                //        response.Result = false;
                //        response.IsSuccessful = false;
                //        response.Error = new List<string>() { "DisplayMessage", "Coupon Price has changed, please confirm" };
                //        return response;
                //    }
                //}

                checkoutHeader.ShoppingCartDetails = cartDto?.ShoppingCartDetail ?? Enumerable.Empty<ShoppingCartDetailDto>();
                
                ////logic to add message to process order.
                //await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

                //////rabbitMQ
                ////_rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");
                //await _cartRepository.ClearCart(checkoutHeader.UserId);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
                return response;
            }

            response.IsSuccessful = true;
            response.Result = true;
            return response;
        }
    }
}
