using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Services.ShoppingCartApi.Models.Entities
{
    [Table("ShoppingCartDetail")]
    public class ShoppingCartDetailEntity 
    {
        [Key]
        public long ShoppingCartDetailId { get; set; }

        public long ShoppingCartHeaderId { get; set; }
        [ForeignKey(nameof(ShoppingCartHeaderId))]
        public virtual ShoppingCartHeaderEntity ShoppingCartHeaderEntity { get; set; }

        public long ProductId { get; set; }
        [ForeignKey(nameof(ProductEntity.ProductId))]
        public virtual ProductEntity ProductEntity { get; set; }

        public int Count { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
