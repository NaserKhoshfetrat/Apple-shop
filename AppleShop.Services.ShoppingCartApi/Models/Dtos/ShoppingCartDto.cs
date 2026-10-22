namespace AppleShop.Services.ShoppingCartApi.Models.Dtos
{
    public class ShoppingCartDto
    {
        public ShoppingCartHeaderDto ShoppingCartHeader { get; set; } = null!;
        public IEnumerable<ShoppingCartDetailDto> ShoppingCartDetail { get; set; } = null!;
    }
}
