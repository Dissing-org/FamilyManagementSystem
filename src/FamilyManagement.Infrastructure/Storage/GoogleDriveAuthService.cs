using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Infrastructure.Storage;

public class GoogleDriveAuthService : IGoogleDriveAuthService
{
    private readonly GoogleDriveOptions _options;
    private readonly ILogger<GoogleDriveAuthService> _logger;
    private GoogleAuthorizationCodeFlow? _flow;

    public GoogleDriveAuthService(
        IOptions<GoogleDriveOptions> options,
        ILogger<GoogleDriveAuthService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public bool IsConfigured =>
        (!string.IsNullOrWhiteSpace(_options.ClientId) && !string.IsNullOrWhiteSpace(_options.ClientSecret))
        || (!string.IsNullOrWhiteSpace(_options.ClientSecretsJsonPath) && File.Exists(_options.ClientSecretsJsonPath));

    public GoogleAuthorizationCodeFlow GetFlow()
    {
        if (_flow is not null)
        {
            return _flow;
        }

        ClientSecrets secrets;
        if (!string.IsNullOrWhiteSpace(_options.ClientSecretsJsonPath) && File.Exists(_options.ClientSecretsJsonPath))
        {
            using var stream = new FileStream(_options.ClientSecretsJsonPath, FileMode.Open, FileAccess.Read);
            secrets = GoogleClientSecrets.FromStream(stream).Secrets;
        }
        else if (!string.IsNullOrWhiteSpace(_options.ClientId) && !string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            };
        }
        else
        {
            throw new InvalidOperationException("Google Drive OAuth is not configured. Set GoogleDrive:ClientId and GoogleDrive:ClientSecret, or GoogleDrive:ClientSecretsJsonPath.");
        }

        var tokenDirectory = Path.Combine(AppContext.BaseDirectory, _options.TokenDataStorePath);
        _flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { DriveService.Scope.DriveFile },
            DataStore = new FileDataStore(tokenDirectory, true),
            Prompt = "consent"
        });

        return _flow;
    }

    public async Task<bool> IsAuthorizedAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            return false;
        }

        try
        {
            var flow = GetFlow();
            var token = await flow.LoadTokenAsync("user", cancellationToken);
            if (token is null)
            {
                return false;
            }

            if (!token.IsStale)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                var refreshed = await flow.RefreshTokenAsync("user", token.RefreshToken, cancellationToken);
                return refreshed is not null && !refreshed.IsStale;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to verify Google Drive OAuth authorization status");
            return false;
        }
    }

    public async Task<DriveService?> TryCreateUserDriveServiceAsync(CancellationToken cancellationToken = default)
    {
        if (!await IsAuthorizedAsync(cancellationToken))
        {
            return null;
        }

        try
        {
            var flow = GetFlow();
            var token = await flow.LoadTokenAsync("user", cancellationToken);
            if (token is null)
            {
                return null;
            }

            var userCredential = new UserCredential(flow, "user", token);
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential,
                ApplicationName = "FamilyManagementSystem"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create UserCredential DriveService");
            return null;
        }
    }

    public string GetAuthorizationUrl(string redirectUri)
    {
        var flow = GetFlow();
        var request = flow.CreateAuthorizationCodeRequest(redirectUri);
        return request.Build().AbsoluteUri;
    }

    public async Task<bool> HandleCallbackAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
    {
        try
        {
            var flow = GetFlow();
            var token = await flow.ExchangeCodeForTokenAsync("user", code, redirectUri, cancellationToken);
            _logger.LogInformation("Successfully exchanged authorization code for Google Drive OAuth token.");
            return token is not null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging Google authorization code for token");
            return false;
        }
    }

    public async Task RevokeAuthorizationAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConfigured) return;

        try
        {
            var flow = GetFlow();
            await flow.DeleteTokenAsync("user", cancellationToken);
            _logger.LogInformation("Revoked and deleted local Google Drive OAuth token.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error deleting Google Drive OAuth token");
        }
    }
}
