using AppleShop.Services.ProductApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.ProductApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<ProductEntity> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name == nameof(ProductEntity.CreatedAt)))
            {
                property.SetDefaultValueSql("(getutcdate())");
            }

            SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var createdAt = new DateTime(2026, 2, 15, 16, 30, 0, DateTimeKind.Utc);
            var fruits = new List<ProductEntity>{
                new ProductEntity
                {
                    ProductId = 1,
                    CreatedAt = createdAt,
                    Name = "Organic Gala Apple",
                    Price = 0.99m,
                    Description = "Sweet and crisp organic Gala apples. Grown without synthetic pesticides.",
                    ImageUrl = "https://picsum.photos/seed/gala-apple/400/300"
                },
                new ProductEntity
                {
                    ProductId = 2,
                    CreatedAt = createdAt,
                    Name = "Banana",
                    Price = 0.59m,
                    Description = "Ripe, yellow bananas – perfect for smoothies or a quick snack.",
                    ImageUrl = "https://picsum.photos/seed/banana/400/300"
                },
                new ProductEntity
                {
                    ProductId = 3,
                    CreatedAt = createdAt,
                    Name = "Organic Strawberries (1 lb)",
                    Price = 4.99m,
                    Description = "Juicy, sweet organic strawberries. Washed and ready to eat.",
                    ImageUrl = "https://picsum.photos/seed/strawberries/400/300"
                },
                new ProductEntity
                {
                    ProductId = 4,
                    CreatedAt = createdAt,
                    Name = "Navel Orange",
                    Price = 0.79m,
                    Description = "Seedless navel oranges – high in Vitamin C.",
                    ImageUrl = "https://picsum.photos/seed/orange/400/300"
                },
                new ProductEntity
                {
                    ProductId = 5,
                    CreatedAt = createdAt,
                    Name = "Red Seedless Grapes (0.5 KG bag)",
                    Price = 3.49m,
                    Description = "Sweet, crunchy red grapes. No seeds, great for snacking.",
                    ImageUrl = "https://picsum.photos/seed/red-grapes/400/300"
                },
                new ProductEntity
                {
                    ProductId = 6,
                    CreatedAt = createdAt,
                    Name = "Honey Mango",
                    Price = 2.49m,
                    Description = "Sweet, aromatic honey mangoes with smooth, fiberless flesh.",
                    ImageUrl = "https://picsum.photos/seed/honey-mango/400/300"
                },
                new ProductEntity
                {
                    ProductId = 7,
                    CreatedAt = createdAt,
                    Name = "Pineapple (whole)",
                    Price = 3.99m,
                    Description = "Sweet golden pineapple – ready to core and slice.",
                    ImageUrl = "https://picsum.photos/seed/pineapple/400/300"
                },
                new ProductEntity
                {
                    ProductId = 8,
                    CreatedAt = createdAt,
                    Name = "Kiwi Fruit (6-pack)",
                    Price = 3.29m,
                    Description = "Zesty green kiwis, rich in vitamin C and fiber.",
                    ImageUrl = "https://picsum.photos/seed/kiwi/400/300"
                },
                new ProductEntity
                {
                    ProductId = 9,
                    CreatedAt = createdAt,
                    Name = "Fresh Blueberries (250 Grams)",
                    Price = 4.49m,
                    Description = "Plump, antioxidant-rich blueberries – perfect for baking or snacking.",
                    ImageUrl = "https://picsum.photos/seed/blueberries/400/300"
                },
                new ProductEntity
                {
                    ProductId = 10,
                    CreatedAt = createdAt,
                    Name = "Bartlett Pear",
                    Price = 1.29m,
                    Description = "Juicy, soft Bartlett pears with a classic sweet flavor.",
                    ImageUrl = "https://picsum.photos/seed/bartlett-pear/400/300"
                },
                new ProductEntity
                {
                    ProductId = 11,
                    CreatedAt = createdAt,
                    Name = "Rainier Cherry (0.5 KG)",
                    Price = 6.99m,
                    Description = "Premium yellow‑red Rainier cherries – very sweet and delicate.",
                    ImageUrl = "https://picsum.photos/seed/rainier-cherry/400/300"
                },
                new ProductEntity
                {
                    ProductId = 12,
                    CreatedAt = createdAt,
                    Name = "Black Plum",
                    Price = 1.49m,
                    Description = "Dark red‑skinned plums with sweet, juicy interior.",
                    ImageUrl = "https://picsum.photos/seed/black-plum/400/300"
                },
                new ProductEntity
                {
                    ProductId = 13,
                    CreatedAt = createdAt,
                    Name = "Frozen Mixed Berry Blend (0.5 KG)",
                    Price = 5.49m,
                    Description = "Strawberries, blueberries, raspberries, blackberries – frozen fresh.",
                    ImageUrl = "https://picsum.photos/seed/mixed-berries/400/300"
                },
                new ProductEntity
                {
                    ProductId = 14,
                    CreatedAt =createdAt,
                    Name = "Organic Cucumber",
                    Price = 1.29m,
                    Description = "Fresh, crisp organic cucumber – perfect for salads or snacking.",
                    ImageUrl = "https://picsum.photos/seed/cucumber/400/300"
                },
                new ProductEntity
                {
                    ProductId = 15,
                    CreatedAt = createdAt,
                    Name = "Fresh Figs (6-pack)",
                    Price = 4.79m,
                    Description = "Soft, honey‑sweet fresh figs – eat as is or pair with cheese.",
                    ImageUrl = "https://picsum.photos/seed/figs/400/300"
                },
            };

            modelBuilder.Entity<ProductEntity>().HasData(fruits);

        }
    }
}