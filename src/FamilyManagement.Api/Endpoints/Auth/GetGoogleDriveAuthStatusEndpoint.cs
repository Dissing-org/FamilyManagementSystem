using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

public class GoogleDriveAuthStatusResponse
{
    public bool IsConfigured { get; set; }
    public bool IsAuthorized { get; set; }
    public string? LoginUrl { get; set; }
}

public class GetGoogleDriveAuthStatusEndpoint : EndpointWithoutRequest<GoogleDriveAuthStatusResponse>
{
    private readonly IGoogleDriveAuthService _authService;
    private readonly ITenantContext _tenantContext;

    public GetGoogleDriveAuthStatusEndpoint(IGoogleDriveAuthService authService, ITenantContext tenantContext)
    {
        _authService = authService;
        _tenantContext = tenantContext;
    }

    public override void Configure()
    {
        Get("/api/auth/google/status");
        Summary(s =>
        {
            s.Summary = "Check Google Drive OAuth authorization status";
            s.Description = "Returns whether OAuth is configured and whether a valid user token is present.";
        });
    }

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
