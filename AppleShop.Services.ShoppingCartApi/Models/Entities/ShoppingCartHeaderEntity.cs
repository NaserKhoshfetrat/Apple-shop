using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Services.ShoppingCartApi.Models.Entities
{
    [Table("ShoppingCartHeader")]
    [Index(nameof(UserId), IsUnique = true)]
    public class ShoppingCartHeaderEntity
    {
        [Key]
        public long Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? CouponCode { get; set; }

    }
}
