using AppleShop.Services.CouponApi.Models.Dtos;
using AppleShop.Services.CouponApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppleShop.Services.CouponApi.Controllers
{
    [Route("api/couponApi")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        public ILogger<CouponApiController> _logger { get; set; }
        private readonly ICouponRepository _couponRepository;


        public CouponApiController(ILogger<CouponApiController> logger, ICouponRepository couponRepository)
        {
            _logger = logger;
            _couponRepository = couponRepository;
        }

        [HttpGet("GetCouponCodeDiscount/{couponCode}")]
        public async Task<BaseResponseDto<CouponResponseDto>> GetCouponCodeDiscount(string couponCode)
        {
            BaseResponseDto<CouponResponseDto> response = new();

            try
            {
                CouponResponseDto dto = await _couponRepository.Read(couponCode);
                response.Result = dto;
            }
            catch (Exception ex)
            {
                response.IsSuccessful = false;
                response.Error = new List<string>() { ex.ToString() };
            }

            return response;
        }
    }
}
