using AppleShop.Library.DAL.Repository.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Services.ProductApi.Models.Entities
{
    [Table("Products")]
    [Index(nameof(Name), IsUnique = true)]
    public class ProductEntity : IEntityMarker
    {
        [Key]
        public long ProductId { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; } = null!;

        [Range(minimum: 0.01, maximum: long.MaxValue, ErrorMessage = "Price must be at least 0.01")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }
    }
}
