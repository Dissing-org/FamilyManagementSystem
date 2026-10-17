namespace FamilyManagement.Api.Auth;

public class AuthOptions
{
    public const string SectionName = "Auth";
    public const string HybridScheme = "HybridAuth";
    public const string DemoCookieScheme = "DemoCookie";
    public const string AuthCookieName = "FamilyHub.Auth";
    public const string SignedOutCookieName = "FamilyHub.SignedOut";

    public string OwnerEmail { get; set; } = "mathi@familyhub.dk";
    public string OwnerDisplayName { get; set; } = "Mathi";
    public string DemoEmail { get; set; } = "demo@familyhub.dk";
    public string DemoPassword { get; set; } = "FamilyHubDemo2026!";
    public string DemoDisplayName { get; set; } = "Guest";
    public bool AllowDevelopmentOwnerFallback { get; set; } = true;
    public int DemoResetIntervalHours { get; set; } = 24;
}
