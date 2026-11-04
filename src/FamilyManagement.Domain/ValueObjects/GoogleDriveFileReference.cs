namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Represents a reference to an external file stored in Google Drive.
/// </summary>
public sealed record GoogleDriveFileReference
{
    /// <summary>
    /// Gets the unique Google Drive file identifier.
    /// </summary>
    public string FileId { get; }

    /// <summary>
    /// Gets the name of the file in Google Drive.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the optional web view URL for viewing the file in a browser.
    /// </summary>
    public string? WebViewLink { get; }

    private GoogleDriveFileReference(string fileId, string fileName, string? webViewLink)
    {
        FileId = fileId;
        FileName = fileName;
        WebViewLink = webViewLink;
    }

    /// <summary>
    /// Creates a new <see cref="GoogleDriveFileReference"/> instance after validating input parameters.
    /// </summary>
    /// <param name="fileId">The unique Google Drive file identifier.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="webViewLink">An optional web view URL for viewing the file.</param>
    /// <returns>A validated <see cref="GoogleDriveFileReference"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fileId"/> or <paramref name="fileName"/> is null or white-space.</exception>
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
