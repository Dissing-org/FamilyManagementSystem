using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using FamilyManagement.Api.Auth;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Endpoint that terminates the current demo session and sets a sign-out marker cookie.
/// </summary>
public class LogoutEndpoint : EndpointWithoutRequest
{
    /// <summary>
    /// Configures the route mapping, anonymous access, and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/auth/logout");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Sign out of the current session";
            s.Description = "Clears the DemoCookie session and sets the signed-out marker cookie.";
        });
    }

    /// <summary>
    /// Handles the sign-out request by clearing the demo cookie and writing the signed-out cookie.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        await HttpContext.SignOutAsync(AuthOptions.DemoCookieScheme);

        HttpContext.Response.Cookies.Append(
            AuthOptions.SignedOutCookieName,
            "true",
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(new { message = "Signed out." }));
    }
}
