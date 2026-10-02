namespace FamilyManagement.Application.Interfaces;

public interface IGoogleDriveAuthService
{
    bool IsConfigured { get; }
    Task<bool> IsAuthorizedAsync(CancellationToken cancellationToken = default);
    string GetAuthorizationUrl(string redirectUri);
    Task<bool> HandleCallbackAsync(string code, string redirectUri, CancellationToken cancellationToken = default);
    Task RevokeAuthorizationAsync(CancellationToken cancellationToken = default);
}
