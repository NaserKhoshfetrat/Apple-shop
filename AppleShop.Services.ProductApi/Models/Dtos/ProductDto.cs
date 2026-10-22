namespace AppleShop.Services.ProductApi.Models.Dtos
{
    public class ProductDto : BaseRequestDto
    {
        public long ProductId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
    }
}
