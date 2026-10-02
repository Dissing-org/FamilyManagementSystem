using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Interfaces;

public record ReceiptUploadMetadata(
    string? Category = null,
    int? Year = null,
    string? Merchant = null);

public interface IReceiptFileStorageService
{
    Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        ReceiptUploadMetadata? metadata,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
}
