using System.Net.Http.Json;
using System.Text.Json;

namespace FamilyManagement.UI.Shared.Services;

public record AuthSessionState
{
    public bool IsAuthenticated { get; init; }
    public string Role { get; init; } = "Owner";
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = "Mathi";
    public string DemoEmailHint { get; init; } = "demo@familyhub.dk";
    public string DemoPasswordHint { get; init; } = "FamilyHubDemo2026!";
    public bool IsLoaded { get; init; }

    public bool IsDemo => string.Equals(Role, "Demo", StringComparison.OrdinalIgnoreCase);
}

public class AuthSessionResponseDto
{
    public bool IsAuthenticated { get; set; }
    public string? Role { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public bool? IsDemo { get; set; }
    public string? DemoEmailHint { get; set; }
    public string? DemoPasswordHint { get; set; }
    public string? DemoEmail { get; set; }
    public string? DemoPassword { get; set; }
}

public interface IAuthSessionService
{
    AuthSessionState Current { get; }
    event Action? OnChange;
    Task<AuthSessionState> RefreshSessionAsync(CancellationToken ct = default);
    Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
    Task<bool> ResetDemoDataAsync(CancellationToken ct = default);
}

public class AuthSessionService : IAuthSessionService
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public AuthSessionState Current { get; private set; } = new();
    public event Action? OnChange;

    public AuthSessionService(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
    {
        _httpClient = httpClient;
        _baseUrlProvider = baseUrlProvider;
    }

    private string BuildUrl(string relativePath) =>
        _baseUrlProvider.ResolveUrl(_httpClient, relativePath);

    public async Task<AuthSessionState> RefreshSessionAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(BuildUrl("api/auth/session"), ct);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<AuthSessionResponseDto>(cancellationToken: ct);
                if (dto is not null)
                {
                    var role = !string.IsNullOrWhiteSpace(dto.Role)
                        ? dto.Role
                        : (dto.IsDemo == true ? "Demo" : "Owner");

                    Current = new AuthSessionState
                    {
                        IsAuthenticated = dto.IsAuthenticated,
                        Role = role,
                        Email = dto.Email ?? string.Empty,
                        DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? "Mathi" : dto.DisplayName,
                        DemoEmailHint = FirstNonEmpty(dto.DemoEmailHint, dto.DemoEmail, "demo@familyhub.dk"),
                        DemoPasswordHint = FirstNonEmpty(dto.DemoPasswordHint, dto.DemoPassword, "FamilyHubDemo2026!"),
                        IsLoaded = true
                    };
                    NotifyStateChanged();
                    return Current;
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Current = Current with { IsAuthenticated = false, IsLoaded = true };
                NotifyStateChanged();
                return Current;
            }
        }
        catch
        {
            // Fall through when offline or unconfigured
        }

        Current = Current with { IsLoaded = true };
        NotifyStateChanged();
        return Current;
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                BuildUrl("api/auth/login"),
                new { email, password },
                ct);

            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<AuthSessionResponseDto>(cancellationToken: ct);
                if (dto is not null && dto.IsAuthenticated)
                {
                    var role = !string.IsNullOrWhiteSpace(dto.Role)
                        ? dto.Role
                        : (dto.IsDemo == true ? "Demo" : "Owner");

                    Current = new AuthSessionState
                    {
                        IsAuthenticated = true,
                        Role = role,
                        Email = dto.Email ?? email,
                        DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? "Guest" : dto.DisplayName,
                        DemoEmailHint = FirstNonEmpty(dto.DemoEmailHint, dto.DemoEmail, Current.DemoEmailHint),
                        DemoPasswordHint = FirstNonEmpty(dto.DemoPasswordHint, dto.DemoPassword, Current.DemoPasswordHint),
                        IsLoaded = true
                    };
                    NotifyStateChanged();
                    return (true, null);
                }

                await RefreshSessionAsync(ct);
                if (Current.IsAuthenticated)
                {
                    return (true, null);
                }
            }

            var errorText = await TryExtractErrorAsync(response, ct);
            return (false, errorText ?? "Invalid email or password.");
        }
        catch (Exception ex)
        {
            return (false, $"Unable to reach authentication server: {ex.Message}");
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            await _httpClient.PostAsync(BuildUrl("api/auth/logout"), null, ct);
        }
        catch
        {
            // Ignore network error on logout
        }

        Current = Current with
        {
            IsAuthenticated = false,
            Role = "Owner",
            Email = string.Empty,
            DisplayName = "Mathi",
            IsLoaded = true
        };
        NotifyStateChanged();
    }

    public async Task<bool> ResetDemoDataAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(BuildUrl("api/demo/reset"), null, ct);
            if (response.IsSuccessStatusCode)
            {
                NotifyStateChanged();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    private static string FirstNonEmpty(params string?[] candidates)
    {
        foreach (var candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate))
            {
                return candidate;
            }
        }
        return string.Empty;
    }

    private static async Task<string?> TryExtractErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("message", out var msgProp))
            {
                return msgProp.GetString();
            }
            if (doc.RootElement.TryGetProperty("error", out var errProp))
            {
                return errProp.GetString();
            }
        }
        catch
        {
            // Ignore JSON parse errors
        }

        return null;
    }
}
