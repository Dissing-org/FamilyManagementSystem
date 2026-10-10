using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using FamilyManagement.Web;
using FamilyManagement.UI.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = builder.Configuration["services:api:https:0"] 
          ?? builder.Configuration["services:api:http:0"] 
          ?? builder.Configuration["ApiSettings:BaseUrl"] 
          ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddSingleton<IApiBaseUrlProvider>(new DefaultApiBaseUrlProvider(apiUrl));

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddScoped(sp =>
{
    var cookieHandler = sp.GetRequiredService<CookieHandler>();
    cookieHandler.InnerHandler = new HttpClientHandler();
    return new HttpClient(cookieHandler) { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
});
builder.Services.AddScoped<IReceiptApiClient, ReceiptApiClient>();
builder.Services.AddScoped<IInsuranceApiClient, InsuranceApiClient>();
builder.Services.AddScoped<ChildApiClient>();
builder.Services.AddScoped<VehicleApiClient>();

await builder.Build().RunAsync();

public class CookieHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}
