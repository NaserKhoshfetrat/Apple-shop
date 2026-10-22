namespace AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.Coupon
{
    public class CouponResponseDto
    {
        public int CouponId { get; set; }

        public string CouponCode { get; set; } = null!;

        public double DiscountAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
