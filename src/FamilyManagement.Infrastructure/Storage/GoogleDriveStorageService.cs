using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Infrastructure.Storage;

public class GoogleDriveStorageService : IReceiptFileStorageService
{
    private readonly GoogleDriveOptions _options;
    private readonly ILogger<GoogleDriveStorageService> _logger;
    private DriveService? _driveService;

    public GoogleDriveStorageService(
        IOptions<GoogleDriveOptions> options,
        ILogger<GoogleDriveStorageService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    private DriveService GetDriveService()
    {
        if (_driveService is not null)
        {
            return _driveService;
        }

        if (string.IsNullOrWhiteSpace(_options.CredentialsJsonPath) || !File.Exists(_options.CredentialsJsonPath))
        {
            throw new InvalidOperationException(
                $"Google Drive credentials JSON file not found at '{_options.CredentialsJsonPath}'. Please configure GoogleDrive:CredentialsJsonPath in appsettings.json.");
        }

        var credential = GoogleCredential.FromFile(_options.CredentialsJsonPath)
            .CreateScoped(DriveService.Scope.DriveFile);

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "FamilyManagementSystem"
        });

        return _driveService;
    }

    public async Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        // Check if fallback to local storage is enabled and credentials are not present
        if ((string.IsNullOrWhiteSpace(_options.CredentialsJsonPath) || !File.Exists(_options.CredentialsJsonPath)) 
            && _options.UseLocalStorageFallbackWhenUnconfigured)
        {
            _logger.LogWarning("Google Drive credentials not found. Using local fallback directory '{Dir}'", _options.LocalStorageFallbackDirectory);
            return await SaveLocallyAsync(fileName, fileStream, cancellationToken);
        }

        var driveService = GetDriveService();

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{fileName}"
        };

        if (!string.IsNullOrWhiteSpace(_options.FolderId))
        {
            fileMetadata.Parents = new List<string> { _options.FolderId };
        }

        var request = driveService.Files.Create(fileMetadata, fileStream, contentType);
        request.Fields = "id, name, webViewLink";

        var uploadProgress = await request.UploadAsync(cancellationToken);
        if (uploadProgress.Status != UploadStatus.Completed)
        {
            throw new InvalidOperationException($"Google Drive upload failed: {uploadProgress.Exception?.Message}", uploadProgress.Exception);
        }

        var uploadedFile = request.ResponseBody;
        _logger.LogInformation("Uploaded receipt '{FileName}' to Google Drive with FileId '{FileId}'", fileName, uploadedFile.Id);

        return GoogleDriveFileReference.Create(
            uploadedFile.Id,
            uploadedFile.Name,
            uploadedFile.WebViewLink);
    }

    public async Task DeleteAsync(string fileId, CancellationToken cancellationToken = default)
    {
        if ((string.IsNullOrWhiteSpace(_options.CredentialsJsonPath) || !File.Exists(_options.CredentialsJsonPath))
            && _options.UseLocalStorageFallbackWhenUnconfigured)
        {
            _logger.LogInformation("Simulated deletion of local file reference '{FileId}'", fileId);
            return;
        }

        var driveService = GetDriveService();
        await driveService.Files.Delete(fileId).ExecuteAsync(cancellationToken);
    }

    private async Task<GoogleDriveFileReference> SaveLocallyAsync(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        var localDir = Path.Combine(AppContext.BaseDirectory, _options.LocalStorageFallbackDirectory);
        Directory.CreateDirectory(localDir);

        var fileId = Guid.NewGuid().ToString("N");
        var savedName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{fileName}";
        var destinationPath = Path.Combine(localDir, $"{fileId}_{savedName}");

        using (var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }
            await stream.CopyToAsync(destStream, cancellationToken);
        }

        return GoogleDriveFileReference.Create(fileId, savedName, $"file://{destinationPath.Replace('\\', '/')}");
    }
}
