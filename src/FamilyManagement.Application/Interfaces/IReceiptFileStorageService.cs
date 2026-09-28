using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Interfaces;

public interface IReceiptFileStorageService
{
    Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string fileId, CancellationToken cancellationToken = default);
}
