using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using FamilyManagement.Api.Auth;

namespace FamilyManagement.Api.Endpoints.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginEndpoint : Endpoint<LoginRequest, AuthSessionResponse>
{
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    public LoginEndpoint(IOptionsMonitor<AuthOptions> authOptions)
    {
        _authOptions = authOptions;
    }

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
