namespace AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart
{
    public class ShoppingCartHeaderDto : ShoppingCartOrderHeaderDto
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public string? CouponCode { get; set; }
    }

    //this class i used in submiting dto
    public class ShoppingCartOrderHeaderDto
    {
        //instead of fetching from service, OrderTotal will be calculated in main project
        public decimal OrderTotal { get; set; }
        public decimal DiscountTotal { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime PickupDateTime { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryBankCardMonthYear { get; set; }
    }
}