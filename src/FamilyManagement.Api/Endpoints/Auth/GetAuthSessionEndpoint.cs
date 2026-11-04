using FastEndpoints;
using Microsoft.Extensions.Options;
using FamilyManagement.Api.Auth;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Response payload containing the current user's authentication and workspace session state.
/// </summary>
public class AuthSessionResponse
{
    public bool IsAuthenticated { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? DemoEmailHint { get; set; }
    public string? DemoPasswordHint { get; set; }
}

/// <summary>
/// Endpoint that retrieves current user authentication session state and demo credentials.
/// </summary>
public class GetAuthSessionEndpoint : EndpointWithoutRequest<AuthSessionResponse>
{
    private readonly ITenantContext _tenantContext;
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAuthSessionEndpoint"/> class.
    /// </summary>
    /// <param name="tenantContext">The tenant context providing user identity and workspace role.</param>
    /// <param name="authOptions">The options monitor providing configured authentication options.</param>
    public GetAuthSessionEndpoint(ITenantContext tenantContext, IOptionsMonitor<AuthOptions> authOptions)
    {
        _tenantContext = tenantContext;
        _authOptions = authOptions;
    }

    /// <summary>
    /// Configures the route mapping, anonymous access, and OpenAPI specification for the endpoint.
    /// </summary>
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

    /// <summary>
    /// Handles the request to retrieve current authentication session details and demo credentials.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
