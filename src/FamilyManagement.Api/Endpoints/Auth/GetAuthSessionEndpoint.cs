using FastEndpoints;
using Microsoft.Extensions.Options;
using FamilyManagement.Api.Auth;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

public class AuthSessionResponse
{
    public bool IsAuthenticated { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? DemoEmailHint { get; set; }
    public string? DemoPasswordHint { get; set; }
}

public class GetAuthSessionEndpoint : EndpointWithoutRequest<AuthSessionResponse>
{
    private readonly ITenantContext _tenantContext;
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    public GetAuthSessionEndpoint(ITenantContext tenantContext, IOptionsMonitor<AuthOptions> authOptions)
    {
        _tenantContext = tenantContext;
        _authOptions = authOptions;
    }

    public override void Configure()
    {
        Get("/api/auth/session");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get current user authentication session and workspace role";
            s.Description = "Returns whether the caller is authenticated as Owner or Demo, along with demo login hints.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var config = _authOptions.CurrentValue;
        var isAuthenticated = _tenantContext.IsAuthenticated;

        var response = new AuthSessionResponse
        {
            IsAuthenticated = isAuthenticated,
            Role = isAuthenticated ? _tenantContext.Role.ToString() : string.Empty,
            Email = isAuthenticated ? _tenantContext.UserEmail : string.Empty,
            DisplayName = isAuthenticated ? _tenantContext.DisplayName : string.Empty,
            DemoEmailHint = config.DemoEmail,
            DemoPasswordHint = config.DemoPassword
        };

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(response));
    }
}
