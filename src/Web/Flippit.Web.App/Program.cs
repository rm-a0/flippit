using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Flippit.Web.App;
using Flippit.Web.App.Services;
using Flippit.Web.BL.Installers;
using Flippit.Web.BL.Mappers;
using Flippit.Web.BL.Facades;
using Flippit.Web.DAL;
using Flippit.Web.DAL.Repositories;
using Flippit.Web.BL;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<UserState>();

// Configure LocalDbOptions from configuration
var localDbOptions = new LocalDbOptions();
builder.Configuration.GetSection("LocalDb").Bind(localDbOptions);
builder.Services.AddSingleton(localDbOptions);

// Get API base URL from configuration
var apiBaseUrl = builder.Configuration.GetValue<string>("Api:BaseUrl") ?? "https://localhost:7175";

// Register HttpClient with base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

// Register authentication service
builder.Services.AddScoped<AuthenticationService>();

// Register API clients
builder.Services.AddScoped<IApiApiClient, ApiApiClient>();
builder.Services.AddScoped<IUsersApiClient, UsersApiClient>();
builder.Services.AddScoped<ICardsApiClient, CardsApiClient>();
builder.Services.AddScoped<ICollectionsApiClient, CollectionsApiClient>();
builder.Services.AddScoped<ICompletedLessonsApiClient, CompletedLessonsApiClient>();
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();

// Register LocalDb and repositories
builder.Services.AddSingleton<LocalDb>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<CardRepository>();
builder.Services.AddSingleton<CollectionRepository>();
builder.Services.AddSingleton<CompletedLessonRepository>();

// Register mappers
builder.Services.AddSingleton<UserMapper>();
builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<CollectionMapper>();
builder.Services.AddSingleton<CompletedLessonMapper>();
builder.Services.AddSingleton<ApiModelMapper>();

// Register facades
builder.Services.AddScoped<IUserFacade, UserFacade>();
builder.Services.AddScoped<ICardFacade, CardFacade>();
builder.Services.AddScoped<ICollectionFacade, CollectionFacade>();
builder.Services.AddScoped<ICompletedLessonFacade, CompletedLessonFacade>();

var app = builder.Build();

// Restore authentication on startup
var authService = app.Services.GetRequiredService<AuthenticationService>();
await authService.RestoreAuthenticationAsync();

// Restore user state if authenticated
var userState = app.Services.GetRequiredService<UserState>();
var userFacade = app.Services.GetRequiredService<IUserFacade>();
var userId = await authService.GetUserIdAsync();
if (userId.HasValue)
{
    try
    {
        var user = await userFacade.GetByIdAsync(userId.Value);
        if (user != null)
        {
            userState.SetUser(user);
        }
    }
    catch
    {
        // Failed to restore user, clear auth
        await authService.ClearTokenAsync();
    }
}

await app.RunAsync();
