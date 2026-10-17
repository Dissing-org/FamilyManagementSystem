using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FamilyManagement.Api.Auth;

public class HybridAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string CfAccessEmailHeader = "Cf-Access-Authenticated-User-Email";
    private readonly IOptionsMonitor<AuthOptions> _authOptions;

    public HybridAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptionsMonitor<AuthOptions> authOptions)
        : base(options, logger, encoder)
    {
        _authOptions = authOptions;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var config = _authOptions.CurrentValue;

        var cookieResult = await Context.AuthenticateAsync(AuthOptions.DemoCookieScheme);
        if (cookieResult.Succeeded && cookieResult.Principal?.Identity?.IsAuthenticated == true)
        {
            return AuthenticateResult.Success(new AuthenticationTicket(cookieResult.Principal, Scheme.Name));
        }

        if (Request.Headers.TryGetValue(CfAccessEmailHeader, out var cfEmailValues))
        {
            var cfEmail = cfEmailValues.FirstOrDefault()?.Trim();
            if (!string.IsNullOrWhiteSpace(cfEmail))
            {
                if (string.Equals(cfEmail, config.OwnerEmail, StringComparison.OrdinalIgnoreCase))
                {
                    var principal = CreatePrincipal(
                        id: "owner",
                        email: cfEmail,
                        displayName: config.OwnerDisplayName,
                        role: "Owner");
                    return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
                }

                return AuthenticateResult.NoResult();
            }
        }

        var isSignedOut = Request.Cookies.ContainsKey(AuthOptions.SignedOutCookieName);
        if (!isSignedOut && config.AllowDevelopmentOwnerFallback)
        {
            var fallbackPrincipal = CreatePrincipal(
                id: "owner-dev",
                email: config.OwnerEmail,
                displayName: config.OwnerDisplayName,
                role: "Owner");
            return AuthenticateResult.Success(new AuthenticationTicket(fallbackPrincipal, Scheme.Name));
        }

        return AuthenticateResult.NoResult();
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }

    private static ClaimsPrincipal CreatePrincipal(string id, string email, string displayName, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, AuthOptions.HybridScheme);
        return new ClaimsPrincipal(identity);
    }
}
