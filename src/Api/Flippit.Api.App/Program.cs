using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flippit.Api.App.Endpoints;
using Flippit.Api.App.Models;
using Flippit.Api.App.Services;
using Flippit.Api.BL.Facades;
using Flippit.Api.BL.Installers;
using Flippit.Api.BL.Mappers;
using Flippit.Api.BL.Services;
using Flippit.Api.DAL.Memory.Installers;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.Collection;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;
using Flippit.IdentityProvider.BL.Installers;
using Flippit.IdentityProvider.DAL;
using Flippit.IdentityProvider.DAL.Entities;
using Flippit.IdentityProvider.DAL.Installers;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityProviderDbContext>(options =>
    options.UseInMemoryDatabase("FlippitIdentityDb"));

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

var DALinstaller = new ApiDALMemoryInstaller();
DALinstaller.Install(builder.Services);

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

builder.Services.AddAuthorization();


ConfigureOpenApiDocuments(builder.Services);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    await DataSeeder.SeedAsync(app.Services);
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
