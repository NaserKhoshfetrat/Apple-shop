using AppleShop.Services.Identity.DbContexts;
using AppleShop.Services.Identity.Models;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AppleShop.Services.Identity.Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<IdentityCustomUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DBInitializer(ApplicationDbContext applicationDbContext, UserManager<IdentityCustomUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            bool isFirstTimeRunning = await roleManager.FindByNameAsync(ProjectConfigurations.Role.Admin) is null;
            if (isFirstTimeRunning)
            {
                //add roles
                await roleManager.CreateAsync(new IdentityRole(ProjectConfigurations.Role.Admin));
                await roleManager.CreateAsync(new IdentityRole(ProjectConfigurations.Role.Customer));
            }
            else
            {
                return;
            }

            //add users
            //todo change credential
            await CreateUserIfNotExists("admin", "admin@apple.shop", "Admin@123", "admin", "admin", "123456789012", ProjectConfigurations.Role.Admin);
            await CreateUserIfNotExists("customer", "customer@apple.shop", "Customer@123", "customer", "customer", "234567890123", ProjectConfigurations.Role.Customer);
        }

        async Task CreateUserIfNotExists(string userName, string email, string password, string firstName, string lastName, string phone, string role)
        {
            var existingUser = await userManager.FindByNameAsync(userName);
            if (existingUser != null) return;

            var user = new IdentityCustomUser
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phone,
                PhoneNumberConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {userName}: {errors}");
            }

            // Add to role
            await userManager.AddToRoleAsync(user, role);

            // Add claims
            await userManager.AddClaimsAsync(user, new[]
            {
            new Claim(JwtClaimTypes.Name, $"{user.UserName} {user.LastName}"),
            new Claim(JwtClaimTypes.GivenName, user.UserName),
            new Claim(JwtClaimTypes.FamilyName, user.LastName),
            new Claim(JwtClaimTypes.Role, role),
        });
        }

        public void Initialize()
        {
            InitializeAsync().GetAwaiter().GetResult();


        }
    }
}
