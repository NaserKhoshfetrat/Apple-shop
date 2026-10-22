using AppleShop.Services.ShoppingCartApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.ShoppingCartApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ShoppingCartHeaderEntity> CartHeaders { get; set; }
        public DbSet<ShoppingCartDetailEntity> CartDetails { get; set; }
    }
}
