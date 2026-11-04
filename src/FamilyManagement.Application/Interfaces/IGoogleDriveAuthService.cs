namespace FamilyManagement.Application.Interfaces;

/// <summary>
/// Service contract for Google Drive OAuth2 authentication and token management.
/// </summary>
public interface IGoogleDriveAuthService
{
    /// <summary>
    /// Gets a value indicating whether Google Drive credentials and API settings are configured.
    /// </summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Checks whether valid Google Drive authorization tokens currently exist.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if authorized; otherwise, <c>false</c>.</returns>
    Task<bool> IsAuthorizedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates the OAuth2 authorization URL for user consent.
    /// </summary>
    /// <param name="redirectUri">The redirect URI to which the authorization server will respond.</param>
    /// <returns>The OAuth2 consent screen URL.</returns>
    string GetAuthorizationUrl(string redirectUri);

    /// <summary>
    /// Exchanges an authorization code received from the callback for access and refresh tokens.
    /// </summary>
    /// <param name="code">The authorization code returned from Google OAuth.</param>
    /// <param name="redirectUri">The redirect URI used when requesting authorization.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if successfully authorized; otherwise, <c>false</c>.</returns>
    Task<bool> HandleCallbackAsync(string code, string redirectUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes current Google Drive authorization tokens and clears saved credentials.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RevokeAuthorizationAsync(CancellationToken cancellationToken = default);
}
