using AppleShop.Services.ShoppingCartApi;
using AppleShop.Services.ShoppingCartApi.DbContexts;
using AppleShop.Services.ShoppingCartApi.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.AddDebug();

#region Add services to the container.

string defaultConnectionString = builder.Configuration.GetConnectionString("AppleShop_ShoppingCartApi_DefaultConnection");
if (string.IsNullOrEmpty(defaultConnectionString))
{
    throw new InvalidOperationException("Connection string 'AppleShop_ShoppingCartApi_DefaultConnection' not found.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString: defaultConnectionString));

builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

//IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", options =>
               {
                   string identityBaseUrl = builder.Configuration["ServiceConfiguration:IdentityApi:BaseUrl"] ?? throw new InvalidOperationException("IdentityApi BaseUrl is not configured.");

                   options.RequireHttpsMetadata = false;   // only for development

                   options.Authority = identityBaseUrl;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateAudience = false,
                       //ValidateIssuer = true,            // default is true – keep it unless you know issuer mismatch
                       //ValidIssuer = identityBaseUrl,    // explicitly set to avoid trailing-slash issues
                       // ValidateLifetime = true,          // default is true – ensures token isn’t expired
                       // ClockSkew = TimeSpan.Zero         // if you want strict expiration (not needed normally)
                   };

                   options.Events = new JwtBearerEvents
                   {
                       OnAuthenticationFailed = context =>
                       {
                           Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                           // Also log the inner exception if any
                           if (context.Exception.InnerException != null)
                               Console.WriteLine($"Inner exception: {context.Exception.InnerException.Message}");
                           return Task.CompletedTask;
                       },
                       OnTokenValidated = context =>
                       {
                           Console.WriteLine("Token validated successfully");
                           return Task.CompletedTask;
                       },
                       OnChallenge = context =>
                       {
                           Console.WriteLine("Challenge invoked – likely because no valid token was provided.");
                           return Task.CompletedTask;
                       },
                   };

               });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", ProjectConfigurations.ClientWebProject.ApiScope);
    });
});

#region Swagger/OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    try
    {
        //// add JWT VerificationCode
        var securityScheme = new OpenApiSecurityScheme
        {
            //Type = SecuritySchemeType.ApiKey,
            //In = ParameterLocation.Header,
            ////Name = "Authorization",
            //Description = @"Enter 'bearer' [space] and your access token **_only_**",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        };

        options.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "AppleShop.Service.ShoppingCartApi",
                Version = "v1",
                Description = $"enviroment:{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!}, version:{builder.Configuration["ServerVersionName"]}"
            });

        options.AddSecurityDefinition("bearer", securityScheme);
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });

        options.CustomOperationIds(apiDescription =>
        {
            return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
        });
    }
    catch (ReflectionTypeLoadException ex)
    {
        foreach (var loadEx in ex.LoaderExceptions)
        {
            Console.WriteLine($"Loader error: {loadEx?.Message}");
            if (loadEx is FileNotFoundException fileEx)
                Console.WriteLine($"Missing assembly: {fileEx.FileName}");
        }
        throw;
    }
});
#endregion Swagger/OpenAPI

#endregion Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();

    #region Swagger/OpenAPI

    app.Use(async (context, next) =>
    {
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
            {
                var header = AuthenticationHeaderValue.Parse(authHeader);
                byte[] inBytes = Convert.FromBase64String(header.Parameter!);

                string[] credentials = Encoding.UTF8.GetString(inBytes).Split(':');

                string username = credentials[0];
                string password = credentials[1];

                if (username.Equals(configuration["Swagger:UiUserName"], StringComparison.OrdinalIgnoreCase) &&
                    password.Equals(configuration["Swagger:UiPassword"]))
                {
                    await next(); // valid – continue pipeline
                    return;
                }
            }

            // Unauthorized – challenge client
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            await next(); // not swagger – continue
        }
    });

    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI(c =>
    {
        c.DisplayOperationId();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppleShop.Services.ShoppingCartApi v1"));
    });
    #endregion Swagger/OpenAPI
}

app.UseHttpsRedirection();
app.Use(async (ctx, next) =>
{
    Console.WriteLine($"[REQ] {ctx.Request.Method} {ctx.Request.Path}");
    await next();
    Console.WriteLine($"[RES] {ctx.Response.StatusCode}");
});
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    Console.WriteLine($"Authorization header: {authHeader ?? "null"}");
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();