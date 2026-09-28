namespace FamilyManagement.Domain.ValueObjects;

public sealed record GoogleDriveFileReference
{
    public string FileId { get; }
    public string FileName { get; }
    public string? WebViewLink { get; }

    private GoogleDriveFileReference(string fileId, string fileName, string? webViewLink)
    {
        FileId = fileId;
        FileName = fileName;
        WebViewLink = webViewLink;
    }

    public static GoogleDriveFileReference Create(string fileId, string fileName, string? webViewLink = null)
    {
        if (string.IsNullOrWhiteSpace(fileId))
        {
            throw new ArgumentException("Google Drive File ID is required.", nameof(fileId));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        return new GoogleDriveFileReference(fileId.Trim(), fileName.Trim(), webViewLink?.Trim());
    }
}
