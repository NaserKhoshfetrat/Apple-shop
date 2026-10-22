using AppleShop.Services.CouponApi.DbContexts;
using AppleShop.Services.CouponApi.Models.Dtos;
using AppleShop.Services.CouponApi.Models.Entities;
using AppleShop.Services.CouponApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.CouponApi.Repository
{
    public interface ICouponRepository
    {
        Task<bool> Create(string couponCode, double discountAmount);
        Task<CouponResponseDto> Read(string couponCode);
        Task<bool> Update(CouponResponseDto couponDto);
        Task<bool> Delete(int id);
    }

    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;

        }

        public async Task<bool> Create(string couponCode, double discountAmount)
        {
            var entity = new CouponEntity()
            {
                CouponCode = couponCode,
                DiscountAmount = discountAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Coupons.AddAsync(entity);
            var affectedRows = await _db.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var entityInDb = await _db.Coupons.FindAsync(id);
            if (entityInDb is null) return false;

            _db.Coupons.Remove(entityInDb);
            var affectedRows = await _db.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<CouponResponseDto> Read(string couponCode)
        {
            var entityInDb = await _db.Coupons.Where(u => u.CouponCode.ToLower() == couponCode.ToLower()).FirstOrDefaultAsync();
            return Mapper.Map(entityInDb);
        }

        public async Task<bool> Update(CouponResponseDto couponDto)
        {
            var entityInDb = await _db.Coupons.FindAsync(couponDto.CouponId);

            if (entityInDb == null) return false;

            entityInDb.CouponCode = couponDto.CouponCode;
            entityInDb.DiscountAmount = couponDto.DiscountAmount;
            entityInDb.CreatedAt = couponDto.CreatedAt;

            _db.Coupons.Update(entityInDb);

            var affectedRows = await _db.SaveChangesAsync();

            return affectedRows > 0;
        }

    }
}
