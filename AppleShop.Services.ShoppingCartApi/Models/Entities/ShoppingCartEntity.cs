using System.ComponentModel.DataAnnotations.Schema;

namespace AppleShop.Services.ShoppingCartApi.Models.Entities
{
    [Table("ShoppingCart")]
    public class ShoppingCartEntity
    {
        public ShoppingCartHeaderEntity ShoppingCartHeaderEntity { get; set; } = null!;
        public IEnumerable<ShoppingCartDetailEntity> ShoppingCartDetailEntities { get; set; } = null!;
    }
}
