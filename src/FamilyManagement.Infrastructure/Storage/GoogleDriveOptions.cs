namespace FamilyManagement.Infrastructure.Storage;

public class GoogleDriveOptions
{
    public const string SectionName = "GoogleDrive";

    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ClientSecretsJsonPath { get; set; }
    public string TokenDataStorePath { get; set; } = "google_drive_tokens";

    public string? CredentialsJsonPath { get; set; }
    public string RootFolderName { get; set; } = "Family Receipts";
    public string? RootFolderId { get; set; }
    public string? FolderId
    {
        get => RootFolderId;
        set => RootFolderId = value;
    }

    public string StorageProvider { get; set; } = "Local";
    public string? LocalStorageDirectory { get; set; }
    public string DemoLocalStorageDirectory { get; set; } = "demo_receipts_storage";
    public bool UseLocalStorageFallbackWhenUnconfigured { get; set; } = true;
    public string LocalStorageFallbackDirectory { get; set; } = "receipts_storage";
}
