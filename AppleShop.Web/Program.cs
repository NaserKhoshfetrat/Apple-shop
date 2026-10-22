using AppleShop.Web;
using AppleShop.Web.Models;
using AppleShop.Web.Services.Integration.Coupon;
using AppleShop.Web.Services.Integration.Product;
using AppleShop.Web.Services.Integration.ShoppingCart;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

_ProjectName = builder.Configuration.GetValue<string>("ProjectName", defaultValue: "AppleShop.Web");

//Get the section
var configSection = builder.Configuration.GetSection("ServiceConfiguration");
//Register it for DI (to work IOptions later)
builder.Services.Configure<ServiceConfiguration>(configSection);
// Bind it to a concrete instance for immediate use
ServiceConfiguration serviceConfig = configSection.Get<ServiceConfiguration>() ?? throw new InvalidOperationException("ServiceConfiguration section is missing.");

// Add services to the container.
builder.Services.AddControllersWithViews();

string productApiBaseUrl = builder.Configuration["ServiceConfiguration:ProductApi:BaseUrl"] ?? throw new InvalidOperationException("ProductApi:BaseUrl is not configured.");
builder.Services
    .AddHttpClient<ProductApiClient>(client =>
    {
        client.BaseAddress = new Uri(productApiBaseUrl);
    })
//.AddHttpMessageHandler<LoggingHandler>()
//.AddPolicyHandler(GetRetryPolicy())
;
builder.Services.AddScoped<IProductServices, ProductServices>();

string shoppingCartApiBaseUrl = serviceConfig.ShoppingCartApi?.BaseUrl ?? throw new InvalidOperationException("ShoppingCartApi:BaseUrl is not configured.");
builder.Services
    .AddHttpClient<ShoppingCartApiClient>(client =>
    {
        client.BaseAddress = new Uri(shoppingCartApiBaseUrl);
    });
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

string couponApiBaseUrl = serviceConfig.CouponApi?.BaseUrl ?? throw new InvalidOperationException("CouponApi:BaseUrl is not configured.");
builder.Services
    .AddHttpClient<CouponApiClient>(client =>
    {
        client.BaseAddress = new Uri(couponApiBaseUrl);
    });
builder.Services.AddScoped<ICouponServices, CouponServices>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["ServiceConfiguration:IdentityApi:BaseUrl"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = ProjectConfigurations.ClientWebProject.ClientId;
        options.ClientSecret = ProjectConfigurations.ClientWebProject.Secret;
        options.ResponseType = "code";
        options.ClaimActions.MapJsonKey("role", "role", "role");
        options.ClaimActions.MapJsonKey("sub", "sub", "sub");
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add(ProjectConfigurations.ClientWebProject.ApiScope);
        options.SaveTokens = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
