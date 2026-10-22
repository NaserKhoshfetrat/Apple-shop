using Microsoft.AspNetCore.Identity;

namespace AppleShop.Services.Identity.Models
{
    public class IdentityCustomUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
