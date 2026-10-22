using System.ComponentModel.DataAnnotations;

namespace AppleShop.Services.ShoppingCartApi.Models.Dtos
{
    public class ProductDto
    {
        public long ProductId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; } = null!;

        [Range(minimum: 0.01, maximum: long.MaxValue, ErrorMessage = "Price must be at least 0.01")]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
