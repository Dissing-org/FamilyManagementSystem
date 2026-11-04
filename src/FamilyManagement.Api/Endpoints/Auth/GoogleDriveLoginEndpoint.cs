using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Endpoint that initiates the Google Drive OAuth authorization flow.
/// </summary>
public class GoogleDriveLoginEndpoint : EndpointWithoutRequest
{
    private readonly IGoogleDriveAuthService _authService;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleDriveLoginEndpoint"/> class.
    /// </summary>
    /// <param name="authService">The Google Drive authentication service.</param>
    /// <param name="tenantContext">The tenant context providing workspace identity.</param>
    public GoogleDriveLoginEndpoint(IGoogleDriveAuthService authService, ITenantContext tenantContext)
    {
        _authService = authService;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/auth/google/login");
        Summary(s =>
        {
            s.Summary = "Initiate Google Drive OAuth login";
            s.Description = "Redirects the user to the Google consent screen.";
        });
    }

    /// <summary>
    /// Handles the request to initiate Google Drive OAuth login by redirecting to Google's consent screen.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        if (_tenantContext.IsDemo)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.Forbid());
            return;
        }

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
