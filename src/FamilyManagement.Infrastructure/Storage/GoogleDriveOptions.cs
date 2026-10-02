namespace FamilyManagement.Infrastructure.Storage;

public class GoogleDriveOptions
{
    public const string SectionName = "GoogleDrive";

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
