using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using FamilyManagement.Api.Auth;

namespace FamilyManagement.Api.Endpoints.Auth;

public class LogoutEndpoint : EndpointWithoutRequest
{
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
