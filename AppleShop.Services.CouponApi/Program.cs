using AppleShop.Services.CouponApi.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.AddDebug();

#region Add services to the container.

builder.ConfigureServices();

builder.Services.AddControllers();

#endregion Add services to the container.

var app = builder.Build();

app.SetupMiddlewares();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();