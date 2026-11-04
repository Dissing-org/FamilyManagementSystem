using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Interfaces;

/// <summary>
/// Metadata providing organizational context when storing a receipt file.
/// </summary>
/// <param name="Category">The optional expense category.</param>
/// <param name="Year">The optional calendar year of the receipt.</param>
/// <param name="Merchant">The optional merchant or vendor name.</param>
public record ReceiptUploadMetadata(
    string? Category = null,
    int? Year = null,
    string? Merchant = null);

/// <summary>
/// Service contract for storing and deleting receipt files in external storage.
/// </summary>
public interface IReceiptFileStorageService
{
    /// <summary>
    /// Uploads a receipt file stream to storage.
    /// </summary>
    /// <param name="fileName">The name of the file to store.</param>
    /// <param name="fileStream">The binary content stream of the file.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="GoogleDriveFileReference"/> for the stored file.</returns>
    Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a receipt file stream to storage with organizational metadata.
    /// </summary>
    /// <param name="fileName">The name of the file to store.</param>
    /// <param name="fileStream">The binary content stream of the file.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="metadata">Optional metadata used to organize and locate the file.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="GoogleDriveFileReference"/> for the stored file.</returns>
    Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        ReceiptUploadMetadata? metadata,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a stored receipt file by its storage identifier.
    /// </summary>
    /// <param name="fileId">The unique identifier of the file in storage.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
}
