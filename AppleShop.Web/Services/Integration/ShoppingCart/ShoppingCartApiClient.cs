using AppleShop.Web.Commons;

namespace AppleShop.Web.Services.Integration.ShoppingCart
{
    public class ShoppingCartApiClient : BaseApiConsumer
    {
        public ShoppingCartApiClient(HttpClient httpClient) : base(httpClient) { }
    }
}
