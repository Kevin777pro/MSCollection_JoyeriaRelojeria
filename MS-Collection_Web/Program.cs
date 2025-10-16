using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MS_Collection_Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//agregar los servicios a ocupar
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44300/") });
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<CarritoService>();


await builder.Build().RunAsync();
