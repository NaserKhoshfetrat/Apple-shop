using AppleShop.Services.ShoppingCartApi.DbContexts;
using AppleShop.Services.ShoppingCartApi.Models.Dtos;
using AppleShop.Services.ShoppingCartApi.Models.Entities;
using AppleShop.Services.ShoppingCartApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.ShoppingCartApi.Repository
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCartDto> GetCartByUserId(string userId);
        Task<ShoppingCartDto> CreateUpdateCart(ShoppingCartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailsId);
        Task<bool> ApplyCoupon(string userId, string couponCode);
        Task<bool> RemoveCoupon(string userId);
        Task<bool> ClearCart(string userId);
    }

    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;

        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            return await UpdateCoupon(userId, couponCode);
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            return await UpdateCoupon(userId, string.Empty);
        }

        private async Task<bool> UpdateCoupon(string userId, string couponCode)
        {
            var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDb != null)
            {
                cartHeaderFromDb.CouponCode = couponCode;
                _db.CartHeaders.Update(cartHeaderFromDb);
                var affectedRows = await _db.SaveChangesAsync();
                return affectedRows > 0;
            }
            else return false;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDb != null)
            {
                _db.CartDetails
                    .RemoveRange(_db.CartDetails.Where(u => u.ShoppingCartHeaderId == cartHeaderFromDb.Id));
                _db.CartHeaders.Remove(cartHeaderFromDb);
                await _db.SaveChangesAsync();
                return true;

            }
            return false;
        }

        public async Task<ShoppingCartDto> CreateUpdateCart(ShoppingCartDto cartDto)
        {
            ShoppingCartEntity cart = cartDto.Map();

            //check if product exists in database, if not create it!
            var prodInDb = await _db.Products
                .FirstOrDefaultAsync(u => u.ProductId == cartDto.ShoppingCartDetail.FirstOrDefault()
                .ProductId);
            if (prodInDb == null)
            {
                _db.Products.Add(cart.ShoppingCartDetailEntities.FirstOrDefault().ProductEntity);
                await _db.SaveChangesAsync();
            }

            /*//create header and details
                _db.CartHeaders.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();*/
            //check if header is null
            var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == cart.ShoppingCartHeaderEntity.UserId);

            if (cartHeaderFromDb == null)
            {
                //create header and details
                _db.CartHeaders.Add(cart.ShoppingCartHeaderEntity);
                await _db.SaveChangesAsync();
                cart.ShoppingCartDetailEntities.FirstOrDefault().ShoppingCartHeaderId = cart.ShoppingCartHeaderEntity.Id;
                cart.ShoppingCartDetailEntities.FirstOrDefault().ProductEntity = null;
                _db.CartDetails.Add(cart.ShoppingCartDetailEntities.FirstOrDefault());
                await _db.SaveChangesAsync();
            }
            else
            {
                //if header is not null
                //check if details has same product
                var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    u => u.ProductId == cart.ShoppingCartDetailEntities.FirstOrDefault().ProductId &&
                    u.ShoppingCartHeaderId == cartHeaderFromDb.Id);

                if (cartDetailsFromDb == null)
                {
                    //create details
                    cart.ShoppingCartDetailEntities.FirstOrDefault().ShoppingCartHeaderId = cartHeaderFromDb.Id;
                    cart.ShoppingCartDetailEntities.FirstOrDefault().ProductEntity = null;
                    _db.CartDetails.Add(cart.ShoppingCartDetailEntities.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //update the count / cart details
                    cart.ShoppingCartDetailEntities.FirstOrDefault().ProductEntity = null;
                    cart.ShoppingCartDetailEntities.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    cart.ShoppingCartDetailEntities.FirstOrDefault().ShoppingCartDetailId = cartDetailsFromDb.ShoppingCartDetailId;
                    cart.ShoppingCartDetailEntities.FirstOrDefault().ShoppingCartHeaderId = cartDetailsFromDb.ShoppingCartHeaderId;
                    _db.CartDetails.Update(cart.ShoppingCartDetailEntities.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }

            return cart.Map();

        }

        public async Task<ShoppingCartDto> GetCartByUserId(string userId)
        {
            ShoppingCartEntity cart = new()
            {
                ShoppingCartHeaderEntity = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)
            };

            cart.ShoppingCartDetailEntities = _db.CartDetails
                .Where(u => u.ShoppingCartHeaderId == cart.ShoppingCartHeaderEntity.Id).Include(u => u.ProductEntity);

            return cart.Map();
        }

        public async Task<bool> RemoveFromCart(int cartDetailsId)
        {
            try
            {
                ShoppingCartDetailEntity cartDetails = await _db.CartDetails
                    .FirstOrDefaultAsync(u => u.ShoppingCartDetailId == cartDetailsId);

                int totalCountOfCartItems = _db.CartDetails
                    .Where(u => u.ShoppingCartHeaderId == cartDetails.ShoppingCartHeaderId).Count();

                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                        .FirstOrDefaultAsync(u => u.Id == cartDetails.ShoppingCartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
