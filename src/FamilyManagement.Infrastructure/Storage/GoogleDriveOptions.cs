namespace FamilyManagement.Infrastructure.Storage;

public class GoogleDriveOptions
{
    public const string SectionName = "GoogleDrive";

    // OAuth 2.0 User Authentication (Recommended for Personal Google Drive)
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientSecretsJsonPath { get; set; }
    public string TokenDataStorePath { get; set; } = "google_drive_tokens";

    // Service Account (for Google Workspace Shared Drives)
    public string? CredentialsJsonPath { get; set; }
    public string RootFolderName { get; set; } = "Family Receipts";
    public string? RootFolderId { get; set; }
    public string? FolderId
    {
        get => RootFolderId;
        set => RootFolderId = value;
    }
    public bool UseLocalStorageFallbackWhenUnconfigured { get; set; } = true;
    public string LocalStorageFallbackDirectory { get; set; } = "receipts_storage";
}
