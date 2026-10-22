namespace AppleShop.Services.ShoppingCartApi.Models.Dtos
{
    public class ShoppingCartHeaderDto
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public string? CouponCode { get; set; }

    }
}
