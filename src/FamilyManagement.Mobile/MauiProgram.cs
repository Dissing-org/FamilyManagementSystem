using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Devices;
using FamilyManagement.UI.Shared.Services;

namespace FamilyManagement.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var danishCulture = new CultureInfo("da-DK");
		CultureInfo.DefaultThreadCurrentCulture = danishCulture;
		CultureInfo.DefaultThreadCurrentUICulture = danishCulture;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		// Default Android emulator loopback is 10.0.2.2, localhost for other platforms
		var defaultApiUrl = DeviceInfo.Platform == DevicePlatform.Android 
			? "http://10.0.2.2:5270" 
			: "http://localhost:5270";

		builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
		builder.Services.AddSingleton<IApiBaseUrlProvider>(new DefaultApiBaseUrlProvider(defaultApiUrl));
		builder.Services.AddScoped(sp => new HttpClient());
		builder.Services.AddScoped<IReceiptApiClient, ReceiptApiClient>();
		builder.Services.AddScoped<IInsuranceApiClient, InsuranceApiClient>();
		builder.Services.AddScoped<ChildApiClient>();
		builder.Services.AddScoped<VehicleApiClient>();
		builder.Services.AddScoped<IAuthSessionService, AuthSessionService>();

		return builder.Build();
	}
}
