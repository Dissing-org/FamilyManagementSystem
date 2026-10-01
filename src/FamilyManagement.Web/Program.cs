using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FamilyManagement.Web;
using FamilyManagement.UI.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = builder.Configuration["services:api:https:0"] 
          ?? builder.Configuration["services:api:http:0"] 
          ?? builder.Configuration["ApiSettings:BaseUrl"] 
          ?? "http://localhost:5271";

builder.Services.AddSingleton<IApiBaseUrlProvider>(new DefaultApiBaseUrlProvider(apiUrl));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IReceiptApiClient, ReceiptApiClient>();

await builder.Build().RunAsync();
