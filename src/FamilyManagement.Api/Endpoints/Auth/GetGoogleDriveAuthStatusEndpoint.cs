using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Response payload containing the Google Drive OAuth configuration and authorization status.
/// </summary>
public class GoogleDriveAuthStatusResponse
{
    public bool IsConfigured { get; set; }
    public bool IsAuthorized { get; set; }
    public string? LoginUrl { get; set; }
}

/// <summary>
/// Endpoint that checks Google Drive OAuth authorization status and provides a login URL if required.
/// </summary>
public class GetGoogleDriveAuthStatusEndpoint : EndpointWithoutRequest<GoogleDriveAuthStatusResponse>
{
    private readonly IGoogleDriveAuthService _authService;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGoogleDriveAuthStatusEndpoint"/> class.
    /// </summary>
    /// <param name="authService">The Google Drive authentication service.</param>
    /// <param name="tenantContext">The tenant context providing workspace role and identity.</param>
    public GetGoogleDriveAuthStatusEndpoint(IGoogleDriveAuthService authService, ITenantContext tenantContext)
    {
        _authService = authService;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/auth/google/status");
        Summary(s =>
        {
            s.Summary = "Check Google Drive OAuth authorization status";
            s.Description = "Returns whether OAuth is configured and whether a valid user token is present.";
        });
    }

    /// <summary>
    /// Handles the request to determine whether Google Drive integration is configured and authorized.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        if (_tenantContext.IsDemo)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(new GoogleDriveAuthStatusResponse
            {
                IsConfigured = false,
                IsAuthorized = false,
                LoginUrl = null
            }));
            return;
        }

        var isConfigured = _authService.IsConfigured;
        var isAuthorized = await _authService.IsAuthorizedAsync(ct);
        string? loginUrl = null;

        if (isConfigured && !isAuthorized)
        {
            var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/auth/google/callback";
            loginUrl = _authService.GetAuthorizationUrl(redirectUri);
        }

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(new GoogleDriveAuthStatusResponse
        {
            IsConfigured = isConfigured,
            IsAuthorized = isAuthorized,
            LoginUrl = loginUrl
        }));
    }
}
