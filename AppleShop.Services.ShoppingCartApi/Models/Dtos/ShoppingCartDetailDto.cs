namespace AppleShop.Services.ShoppingCartApi.Models.Dtos
{
    public class ShoppingCartDetailDto
    {
        public long ShoppingCartDetailId { get; set; }

        public long ShoppingCartHeaderId { get; set; }
        public ShoppingCartHeaderDto? ShoppingCartHeader { get; set; }

        public long ProductId { get; set; }
        public ProductDto Product { get; set; }

        public int Count { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
