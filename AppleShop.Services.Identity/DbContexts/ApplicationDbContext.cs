using AppleShop.Services.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.Identity.DbContexts
{
    public class ApplicationDbContext : IdentityDbContext<IdentityCustomUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
