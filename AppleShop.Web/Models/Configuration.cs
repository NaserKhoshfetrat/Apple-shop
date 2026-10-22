namespace AppleShop.Web.Models
{
    public class ServiceConfiguration
    {
        public ProductApi ProductApi { get; set; } = null!;
        public ShoppingCartApi ShoppingCartApi { get; set; } = null!;
        public CouponApi CouponApi { get; set; } = null!;
        public IdentityApi IdentityApi { get; set; } = null!;
    }
    
    public class IdentityApi {
        public string BaseUrl { get; set; } = null!;
    }

    public class ProductApi
    {
        public string BaseUrl { get; set; } = null!;
        public string GetProductsRelativeUrl { get; set; } = null!;
        public string CreateRelativeUrl { get; set; } = null!;
        public string DeleteRelativeUrl { get; set; } = null!;
        public string GetProductDetailRelativeUrl { get; set; } = null!;
        public string UpdateRelativeUrl { get; set; } = null!;
    }
 
    public class ShoppingCartApi
    {
        public string BaseUrl { get; set; } = null!;
        public string GetCartRelativeUrl { get; set; } = null!;
        public string AddCartRelativeUrl { get; set; } = null!;
        public string UpdateCartRelativeUrl { get; set; } = null!;
        public string RemoveCartRelativeUrl { get; set; } = null!;
        public string ApplyCouponRelativeUrl { get; set; } = null!;
        public string RemoveCouponRelativeUrl { get; set; } = null!;
        public string CheckoutRelativeUrl { get; set; } = null!;
    }
 
    public class CouponApi
    {
        public string BaseUrl { get; set; } = null!;
        public string GetCouponRelativeUrl { get; set; } = null!;
    }
}