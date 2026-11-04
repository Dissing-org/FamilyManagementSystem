using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using FamilyManagement.Api.Auth;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Request payload containing demo login credentials.
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Endpoint that validates demo credentials and issues an HttpOnly demo session cookie.
/// </summary>
public class LoginEndpoint : Endpoint<LoginRequest, AuthSessionResponse>
{
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginEndpoint"/> class.
    /// </summary>
    /// <param name="authOptions">The options monitor providing configured demo credentials.</param>
    public LoginEndpoint(IOptionsMonitor<AuthOptions> authOptions)
    {
        _authOptions = authOptions;
    }

    /// <summary>
    /// Configures the route mapping, anonymous access, and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Sign in to the interactive demo workspace";
            s.Description = "Validates demo credentials and issues an HttpOnly authentication cookie.";
        });
    }

    /// <summary>
    /// Handles the demo login request, verifying credentials and issuing a persistent authentication ticket.
    /// </summary>
    /// <param name="req">The login request containing email and password credentials.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var config = _authOptions.CurrentValue;
        var emailMatches = string.Equals(req.Email?.Trim(), config.DemoEmail, StringComparison.OrdinalIgnoreCase);

        var providedBytes = Encoding.UTF8.GetBytes(req.Password ?? string.Empty);
        var expectedBytes = Encoding.UTF8.GetBytes(config.DemoPassword ?? string.Empty);
        var passwordMatches = CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);

        if (!emailMatches || !passwordMatches)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await HttpContext.Response.WriteAsJsonAsync(new { message = "Invalid email or password." }, ct);
            return;
        }

        HttpContext.Response.Cookies.Delete(AuthOptions.SignedOutCookieName);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "demo"),
            new Claim(ClaimTypes.Email, config.DemoEmail),
            new Claim(ClaimTypes.Name, config.DemoDisplayName),
            new Claim(ClaimTypes.Role, "Demo")
        };

        var identity = new ClaimsIdentity(claims, AuthOptions.DemoCookieScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            AuthOptions.DemoCookieScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        var response = new AuthSessionResponse
        {
            IsAuthenticated = true,
            Role = "Demo",
            Email = config.DemoEmail,
            DisplayName = config.DemoDisplayName,
            DemoEmailHint = config.DemoEmail,
            DemoPasswordHint = config.DemoPassword
        };

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(response));
    }
}
