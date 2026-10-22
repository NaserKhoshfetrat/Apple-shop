using AppleShop.Services.CouponApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.CouponApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<CouponEntity> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name == nameof(CouponEntity.CreatedAt)))
            {
                property.SetDefaultValueSql("(getutcdate())");
            }

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var createdAt = new DateTime(2026, 2, 15, 16, 30, 0, DateTimeKind.Utc);
            var fruits = new List<CouponEntity>{
                new CouponEntity
                {
                    CouponId = 1,
                    CreatedAt = createdAt,
                    CouponCode = "10off",
                    DiscountAmount = 10d,
                },
                new CouponEntity
                {
                    CouponId = 2,
                    CreatedAt = createdAt,
                    CouponCode = "20off",
                    DiscountAmount = 20d,
                }
            };

            modelBuilder.Entity<CouponEntity>().HasData(fruits);

        }
    }
}
