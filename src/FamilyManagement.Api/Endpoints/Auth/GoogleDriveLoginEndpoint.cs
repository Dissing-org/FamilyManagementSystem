using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

public class GoogleDriveLoginEndpoint : EndpointWithoutRequest
{
    private readonly IGoogleDriveAuthService _authService;

    public GoogleDriveLoginEndpoint(IGoogleDriveAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Get("/api/auth/google/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Initiate Google Drive OAuth login";
            s.Description = "Redirects the user to the Google consent screen.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_authService.IsConfigured)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest("Google Drive OAuth is not configured. Please set GoogleDrive:ClientId and GoogleDrive:ClientSecret in appsettings.json."));
            return;
        }

        var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/auth/google/callback";
        var authUrl = _authService.GetAuthorizationUrl(redirectUri);

        await HttpContext.Response.SendResultAsync(TypedResults.Redirect(authUrl));
    }
}
