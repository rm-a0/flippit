using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

// Configure IdentityProviderDbContext with InMemory database
// This replaces the DAL installer to avoid SQL Server dependency
builder.Services.AddDbContext<IdentityProviderDbContext>(options =>
    options.UseInMemoryDatabase("FlippitIdentityDb"));

// Register repositories from IdentityProvider DAL
builder.Services.AddTransient<Flippit.IdentityProvider.DAL.Repositories.IAppUserRepository, Flippit.IdentityProvider.DAL.Repositories.AppUserRepository>();

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<AppUserEntity, AppRoleEntity>(options =>
{
    // Password settings for development
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<IdentityProviderDbContext>()
.AddDefaultTokenProviders();

// Install Identity BL components (facades, mappers)
var identityBLInstaller = new IdentityProviderBLInstaller();
identityBLInstaller.Install(builder.Services);

var DALinstaller = new ApiDALMemoryInstaller();
DALinstaller.Install(builder.Services);

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<CollectionMapper>();
builder.Services.AddSingleton<CompletedLessonMapper>();

// Register CurrentUserService and dependencies
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

// Register JWT token service
builder.Services.AddScoped<JwtTokenService>();

// Add authentication and authorization
// In Testing environment, the test factory will override this configuration
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
    // In Testing environment, authentication is configured by the test factory
    builder.Services.AddAuthentication();
}

builder.Services.AddAuthorization();


ConfigureOpenApiDocuments(builder.Services);

var app = builder.Build();

// Seed data (roles and users) - only in non-Testing environments
if (!app.Environment.IsEnvironment("Testing"))
{
    await DataSeeder.SeedAsync(app.Services);
}


if (app.Environment.IsDevelopment())
{

    app.UseOpenApi();
    app.UseSwaggerUi();

}

// Disable HTTPS redirection in Testing environment to avoid warnings in tests
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

    UseAuthEndPoints(endPointsBase);
    UseUserEndPoints(endPointsBase);
    UseCardEndPoints(endPointsBase);
    UseCollectionEndPoints(endPointsBase);
    UseCompletedLessonEndPoints(endPointsBase);
}

void UseAuthEndPoints(RouteGroupBuilder routeGroupBuilder)
{
    var authEndPoints = routeGroupBuilder.MapGroup("auth")
        .WithTags("authentication");

    authEndPoints.MapPost("/login", async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, BadRequest<string>>> (
        [FromBody] LoginRequest request,
        [FromServices] UserManager<AppUserEntity> userManager,
        [FromServices] JwtTokenService jwtTokenService) =>
    {
        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        {
            return TypedResults.BadRequest("Username and password are required");
        }

        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return TypedResults.Unauthorized();
        }

        var token = await jwtTokenService.GenerateTokenAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        var response = new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Roles = roles
        };

        return TypedResults.Ok(response);
    }).AllowAnonymous();
}

void UseUserEndPoints(RouteGroupBuilder routeGroupBuilder)
{
    var userEndPoints = routeGroupBuilder.MapGroup("users")
        .WithTags("users");

    userEndPoints.MapGet("", ([FromServices] IUserFacade userFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) => {
        var users = userFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(users);
    });

    userEndPoints.MapGet("/{id:guid}",
        Results<Ok<UserDetailModel>, NotFound<string>> (
            Guid id,
            [FromServices] IUserFacade userFacade
        )
        =>
        userFacade.GetById(id) is { } user
            ? TypedResults.Ok(user)
            : TypedResults.NotFound($"User with ID {id} was not found.")
    );

    userEndPoints.MapGet("/{id:guid}/collections", (ICollectionFacade collectionFacade, Guid id, [FromQuery] string ? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string ? sortBy = null) =>
    {
        var collections = collectionFacade.SearchByCreatorId(id, filter, sortBy, page, pageSize);
        return TypedResults.Ok(collections);
    });

    userEndPoints.MapGet("/{userId:guid}/CompletedLessons", (ICompletedLessonFacade facade, Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var AllCompletedLessons = facade.SearchByCreatorId(userId, sortBy, page, pageSize);
        return TypedResults.Ok(AllCompletedLessons);
    });

    userEndPoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.Create(model);
        return TypedResults.Ok(id);
    });

    userEndPoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.Update(model);
        return TypedResults.Ok(id);
    });

    userEndPoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    });

    userEndPoints.MapDelete("{id:guid}", (Guid id, IUserFacade userFacade) =>
    {
        userFacade.Delete(id);
    });

}
void UseCardEndPoints(RouteGroupBuilder routeGroupBuilder)
{
    var cardEndPoints = routeGroupBuilder.MapGroup("cards")
        .WithTags("cards");

    cardEndPoints.MapGet("", (ICardFacade cardFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var cards = cardFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(cards);
    });

    cardEndPoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.Create(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    cardEndPoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.Update(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    cardEndPoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    cardEndPoints.MapGet("/{id:guid}",
       Results<Ok<CardDetailModel>, NotFound<string>> (
           Guid id,
           [FromServices] ICardFacade cardFacade
       )
       =>
       cardFacade.GetById(id) is { } card
           ? TypedResults.Ok(card)
           : TypedResults.NotFound($"Card with ID {id} was not found.")
    );

    cardEndPoints.MapDelete("/{id:guid}", (ICardFacade cardFacade, Guid id) =>
    {
        cardFacade.Delete(id);
    }).RequireAuthorization();

}
void UseCollectionEndPoints(RouteGroupBuilder routeGroupBuilder)
{
    var collectionsEndPoints = routeGroupBuilder.MapGroup("collections")
        .WithTags("collections");

    collectionsEndPoints.MapGet("", (ICollectionFacade collectionFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var collections = collectionFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(collections);
    });

    collectionsEndPoints.MapGet("/{id:guid}/cards", (ICardFacade collectionFacade, Guid id, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var cards = collectionFacade.SearchByCollectionId(id, filter, sortBy, page, pageSize);
        return TypedResults.Ok(cards);
    });

    collectionsEndPoints.MapGet("/{id:guid}/CompletedLessons", (ICompletedLessonFacade facade, Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var AllCompletedLessons = facade.SearchByCollectionId(id, sortBy, page, pageSize);
        return TypedResults.Ok(AllCompletedLessons);
    });

    collectionsEndPoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.Create(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    collectionsEndPoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.Update(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    collectionsEndPoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    collectionsEndPoints.MapGet("/{id:guid}",
       Results<Ok<CollectionDetailModel>, NotFound<string>> (
           Guid id,
           [FromServices] ICollectionFacade collectionFacade
       )
       =>
       collectionFacade.GetById(id) is { } card
           ? TypedResults.Ok(card)
           : TypedResults.NotFound($"Collection with ID {id} was not found.")
    );

    collectionsEndPoints.MapDelete("/{id:guid}", (ICollectionFacade collectionFacade, Guid id) =>
    {
        collectionFacade.Delete(id);
    }).RequireAuthorization();
}
void UseCompletedLessonEndPoints(RouteGroupBuilder routeGroupBuilder)
{
    var completedLessonsEndPoints = routeGroupBuilder.MapGroup("completedLessons")
        .WithTags("completedLessons");

    completedLessonsEndPoints.MapGet("", (ICompletedLessonFacade facade, [FromQuery] int pageSize = 10, [FromQuery] int page = 1, [FromQuery] string? filter = null, [FromQuery] string? sortBy = null) =>
    {
        var lessons = facade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(lessons);
    });

    completedLessonsEndPoints.MapDelete("{id:guid}", (ICompletedLessonFacade facade, Guid id) =>
    {
        facade.Delete(id);
    }).RequireAuthorization();

    completedLessonsEndPoints.MapPost("", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.Create(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    completedLessonsEndPoints.MapPut("", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.Update(model);
        return TypedResults.Ok(id);
    }).RequireAuthorization();

    completedLessonsEndPoints.MapPost("upsert", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.CreateOrUpdate(model);
        return TypedResults.Ok(id);

    }).RequireAuthorization();

    completedLessonsEndPoints.MapGet("{id:guid}", (ICompletedLessonFacade facade, Guid id) =>
    {
        var lesson = facade.GetById(id);
        return TypedResults.Ok(lesson);
    });
}


async Task<Dictionary<string, string[]>?> ValidateModelAsync<T>(T model, IValidator<T> validator)
{
    var validationResult = await validator.ValidateAsync(model);

    if (validationResult.IsValid)
    {
        return null;
    }

    return validationResult.Errors
        .GroupBy(e => e.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
}

public partial class Program
{
}
