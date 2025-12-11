using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flippit.Api.App.Endpoints;
using Flippit.Api.App.Services;
using Flippit.Api.BL.Installers;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Api.DAL.Memory.Installers;
using Flippit.Api.DAL.EF.Installers;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.Collection;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;
using Flippit.IdentityProvider.BL.Installers;
using Flippit.IdentityProvider.DAL;
using Flippit.IdentityProvider.DAL.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityConnection");

if (builder.Environment.IsEnvironment("Testing"))
{
    // Use InMemory database for testing
    builder.Services.AddDbContext<IdentityProviderDbContext>(options =>
        options.UseInMemoryDatabase("FlippitIdentityDb"));
}
else
{
    // Use SQL Server for development and production
    if (string.IsNullOrEmpty(identityConnectionString))
    {
        throw new InvalidOperationException("Identity connection string not configured");
    }
    builder.Services.AddDbContext<IdentityProviderDbContext>(options =>
        options.UseSqlServer(identityConnectionString));
}

builder.Services.AddTransient<Flippit.IdentityProvider.DAL.Repositories.IAppUserRepository, Flippit.IdentityProvider.DAL.Repositories.AppUserRepository>();

builder.Services.AddIdentity<AppUserEntity, AppRoleEntity>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<IdentityProviderDbContext>()
.AddDefaultTokenProviders();

var identityBLInstaller = new IdentityProviderBLInstaller();
identityBLInstaller.Install(builder.Services);

// Configure Database Provider for User Data
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "InMemory";

if (builder.Environment.IsEnvironment("Testing") || databaseProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
{
    // Use InMemory database
    var DALinstaller = new ApiDALMemoryInstaller();
    DALinstaller.Install(builder.Services);
}
else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    // Use SQL Server database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("DefaultConnection connection string not configured for SqlServer provider");
    }
    var efInstaller = new ApiDALEFInstaller();
    efInstaller.Install(builder.Services, connectionString);
}
else
{
    throw new InvalidOperationException($"Invalid DatabaseProvider: {databaseProvider}. Supported values are 'InMemory' or 'SqlServer'");
}

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<CollectionMapper>();
builder.Services.AddSingleton<CompletedLessonMapper>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var BLInstaller = new ApiBLInstaller();
BLInstaller.Install(builder.Services);

builder.Services.AddScoped<IValidator<UserDetailModel>, UserDetailModelValidator>();
builder.Services.AddScoped<IValidator<CardDetailModel>, CardDetailModelValidator>();
builder.Services.AddScoped<IValidator<CollectionDetailModel>, CollectionDetailModelValidator>();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7267")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<JwtTokenService>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
}
else
{
    builder.Services.AddAuthentication();
}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddSingleton<IAuthorizationHandler, Flippit.Api.App.Authorization.ResourceOwnerAuthorizationHandler>();


ConfigureOpenApiDocuments(builder.Services);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    await DataSeeder.SeedAsync(app.Services, databaseProvider);
}


if (app.Environment.IsDevelopment())
{

    app.UseOpenApi();
    app.UseSwaggerUi();

}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

UseEndPoints(app);

app.Run();

void ConfigureOpenApiDocuments(IServiceCollection serviceCollection)
{
    serviceCollection.AddEndpointsApiExplorer();
    serviceCollection.AddOpenApiDocument(config =>
    {
        config.Title = "Flippit";
    });
}

void UseEndPoints(WebApplication application)
{
    var endPointsBase = application.MapGroup("api")
        .WithOpenApi();

    endPointsBase.MapAuthEndpoints();
    endPointsBase.MapUserEndpoints();
    endPointsBase.MapCardEndpoints();
    endPointsBase.MapCollectionEndpoints();
    endPointsBase.MapCompletedLessonEndpoints();
}

public partial class Program
{
}
