namespace AppleShop.Web.Models.ProductService.Dtos.IntegratedServices.ShoppingCart
{
    public class ShoppingCartDto
    {
        public ShoppingCartHeaderDto ShoppingCartHeader { get; set; } = null!;
        public IEnumerable<ShoppingCartDetailDto> ShoppingCartDetail { get; set; } = null!;
    }
}
