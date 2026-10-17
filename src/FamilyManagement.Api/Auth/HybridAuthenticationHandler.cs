using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FamilyManagement.Api.Auth;

public class HybridAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string CfAccessEmailHeader = "Cf-Access-Authenticated-User-Email";
    public const string CfAuthorizationCookieName = "CF_Authorization";
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
        var cfEmail = ExtractCloudflareAccessEmail();

        if (!string.IsNullOrWhiteSpace(cfEmail))
        {
            if (!string.Equals(cfEmail, config.DemoEmail, StringComparison.OrdinalIgnoreCase))
            {
                var principal = CreatePrincipal(
                    id: "owner",
                    email: cfEmail,
                    displayName: config.OwnerDisplayName,
                    role: "Owner");
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }

            var demoCookieResult = await Context.AuthenticateAsync(AuthOptions.DemoCookieScheme);
            if (demoCookieResult.Succeeded && demoCookieResult.Principal?.Identity?.IsAuthenticated == true)
            {
                return AuthenticateResult.Success(new AuthenticationTicket(demoCookieResult.Principal, Scheme.Name));
            }

            return AuthenticateResult.NoResult();
        }

        var cookieResult = await Context.AuthenticateAsync(AuthOptions.DemoCookieScheme);
        if (cookieResult.Succeeded && cookieResult.Principal?.Identity?.IsAuthenticated == true)
        {
            return AuthenticateResult.Success(new AuthenticationTicket(cookieResult.Principal, Scheme.Name));
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

    private string? ExtractCloudflareAccessEmail()
    {
        if (Request.Headers.TryGetValue(CfAccessEmailHeader, out var cfEmailValues))
        {
            var headerEmail = cfEmailValues.FirstOrDefault()?.Trim();
            if (!string.IsNullOrWhiteSpace(headerEmail))
            {
                return headerEmail;
            }
        }

        if (Request.Cookies.TryGetValue(CfAuthorizationCookieName, out var cfJwt) &&
            !string.IsNullOrWhiteSpace(cfJwt))
        {
            var parts = cfJwt.Split('.');
            if (parts.Length >= 2)
            {
                try
                {
                    var payloadSegment = parts[1].Replace('-', '+').Replace('_', '/');
                    switch (payloadSegment.Length % 4)
                    {
                        case 2:
                            payloadSegment += "==";
                            break;
                        case 3:
                            payloadSegment += "=";
                            break;
                    }

                    var jsonBytes = Convert.FromBase64String(payloadSegment);
                    using var doc = JsonDocument.Parse(jsonBytes);
                    if (doc.RootElement.TryGetProperty("email", out var emailProp) &&
                        emailProp.ValueKind == JsonValueKind.String)
                    {
                        var cookieEmail = emailProp.GetString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(cookieEmail))
                        {
                            return cookieEmail;
                        }
                    }
                }
                catch
                {
                    // Ignore malformed CF_Authorization cookie
                }
            }
        }

        return null;
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
