using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

public class GoogleDriveCallbackRequest
{
    [QueryParam]
    public string? Code { get; set; }

    [QueryParam]
    public string? Error { get; set; }
}

public class GoogleDriveCallbackEndpoint : Endpoint<GoogleDriveCallbackRequest>
{
    private readonly IGoogleDriveAuthService _authService;

    public GoogleDriveCallbackEndpoint(IGoogleDriveAuthService authService)
    {
        _authService = authService;
    }

    public override void Configure()
    {
        Get("/api/auth/google/callback");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Handle Google OAuth callback";
            s.Description = "Exchanges authorization code for access and refresh tokens.";
        });
    }

    public override async Task HandleAsync(GoogleDriveCallbackRequest req, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(req.Error))
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest($"Google authorization failed: {req.Error}"));
            return;
        }

        if (string.IsNullOrWhiteSpace(req.Code))
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest("Authorization code was missing from callback."));
            return;
        }

        var redirectUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/auth/google/callback";
        var success = await _authService.HandleCallbackAsync(req.Code, redirectUri, ct);

        if (!success)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.Problem("Failed to exchange authorization code for Google token.", statusCode: 500));
            return;
        }

        var html = """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8" />
            <title>Google Drive Connected</title>
            <style>
                body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; display: flex; align-items: center; justify-content: center; height: 100vh; margin: 0; background-color: #f8fafc; }
                .card { background: white; padding: 2.5rem; border-radius: 1rem; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1); text-align: center; max-width: 480px; }
                .icon { font-size: 3rem; margin-bottom: 1rem; color: #16a34a; }
                h1 { color: #0f172a; margin: 0 0 0.5rem 0; font-size: 1.5rem; }
                p { color: #64748b; line-height: 1.5; margin-bottom: 1.5rem; }
                .button { background-color: #2563eb; color: white; padding: 0.75rem 1.5rem; text-decoration: none; border-radius: 0.5rem; font-weight: 500; display: inline-block; }
            </style>
        </head>
        <body>
            <div class="card">
                <div class="icon">&#10004;</div>
                <h1>Google Drive Connected!</h1>
                <p>Your personal Google account has been authorized. Receipts will now be saved directly to your Google Drive.</p>
                <a href="/" class="button">Go to Dashboard</a>
            </div>
        </body>
        </html>
        """;

        await HttpContext.Response.SendResultAsync(TypedResults.Text(html, "text/html"));
    }
}
