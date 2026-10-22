using AppleShop.Web.Commons;

namespace AppleShop.Web.Services.Integration.Product
{
    public class ProductApiClient : BaseApiConsumer
    {
        public ProductApiClient(HttpClient httpClient) : base(httpClient) { }
    }
}
