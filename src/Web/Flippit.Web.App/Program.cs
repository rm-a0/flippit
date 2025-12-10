using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Flippit.Web.App;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<UserState>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7175") });

await builder.Build().RunAsync();
