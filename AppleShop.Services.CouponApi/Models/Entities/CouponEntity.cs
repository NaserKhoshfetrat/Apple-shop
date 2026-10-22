using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Services.CouponApi.Models.Entities
{
    [Table("Coupons")]
    [Index(nameof(CouponCode), IsUnique = true)]
    public class CouponEntity
    {
        [Key]
        public int CouponId { get; set; }

        public string CouponCode { get; set; } = null!;

        public double DiscountAmount { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}
