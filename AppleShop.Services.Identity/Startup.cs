using AppleShop.Services.Identity.DbContexts;
using AppleShop.Services.Identity.Initializer;
using AppleShop.Services.Identity.Models;
using AppleShop.Services.Identity.Services;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Services.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string identityBaseUrl = Configuration.GetValue<string>("ServiceConfiguration:ClientApplication:BaseUrl") ?? throw new InvalidOperationException("ClientApplication BaseUrl is not configured.");

            string? defaultConnectionString = Configuration.GetConnectionString("AppleShop_IdentityServer_DefaultConnection");
            if (string.IsNullOrEmpty(defaultConnectionString))
            {
                throw new InvalidOperationException("Connection string 'AppleShop_IdentityServer_DefaultConnection' not found.");
            }

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString: defaultConnectionString));
            
            services.AddIdentity<IdentityCustomUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            var identityServerBuilder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            }).AddInMemoryIdentityResources(ProjectConfigurations.IdentityResources)
            .AddInMemoryApiScopes(ProjectConfigurations.ApiScopes)
            .AddInMemoryClients(ProjectConfigurations.GetClients(identityBaseUrl))
            //.AddTestUsers(TestUsers.Users)
            .AddAspNetIdentity<IdentityCustomUser>();

            services.AddScoped<IDBInitializer, DBInitializer>();

            services.AddScoped<IProfileService, ProfileService>();
            identityServerBuilder.AddDeveloperSigningCredential();
            

            //services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDBInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            dbInitializer.Initialize();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapStaticAssets();
                endpoints.MapRazorPages()
                .WithStaticAssets();
            });
        }
    }
}
