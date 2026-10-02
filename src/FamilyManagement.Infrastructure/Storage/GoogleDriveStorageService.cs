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

    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _folderIdCache = new();

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

    public Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        return UploadAsync(fileName, fileStream, contentType, metadata: null, cancellationToken);
    }

    public async Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        ReceiptUploadMetadata? metadata,
        CancellationToken cancellationToken = default)
    {
        // Check if fallback to local storage is enabled and credentials are not present
        if ((string.IsNullOrWhiteSpace(_options.CredentialsJsonPath) || !File.Exists(_options.CredentialsJsonPath)) 
            && _options.UseLocalStorageFallbackWhenUnconfigured)
        {
            _logger.LogWarning("Google Drive credentials not found. Using local fallback directory '{Dir}'", _options.LocalStorageFallbackDirectory);
            return await SaveLocallyAsync(fileName, fileStream, metadata, cancellationToken);
        }

        var driveService = GetDriveService();

        // 1. Resolve Root Folder
        var rootFolderName = string.IsNullOrWhiteSpace(_options.RootFolderName) ? "Family Receipts" : _options.RootFolderName;
        var rootFolderId = !string.IsNullOrWhiteSpace(_options.RootFolderId)
            ? _options.RootFolderId
            : await GetOrCreateFolderAsync(driveService, rootFolderName, parentFolderId: null, cancellationToken);

        // 2. Resolve Year Subfolder (e.g. "2026")
        var year = (metadata?.Year ?? DateTime.UtcNow.Year).ToString();
        var yearFolderId = await GetOrCreateFolderAsync(driveService, year, rootFolderId, cancellationToken);

        // 3. Resolve Category Subfolder (e.g. "Groceries")
        var category = SanitizeFolderName(metadata?.Category);
        var targetFolderId = await GetOrCreateFolderAsync(driveService, category, yearFolderId, cancellationToken);

        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{fileName}",
            Parents = new List<string> { targetFolderId }
        };

        var request = driveService.Files.Create(fileMetadata, fileStream, contentType);
        request.Fields = "id, name, webViewLink";

        var uploadProgress = await request.UploadAsync(cancellationToken);
        if (uploadProgress.Status != UploadStatus.Completed)
        {
            throw new InvalidOperationException($"Google Drive upload failed: {uploadProgress.Exception?.Message}", uploadProgress.Exception);
        }

        var uploadedFile = request.ResponseBody;
        _logger.LogInformation(
            "Uploaded receipt '{FileName}' to Google Drive in folder '{Root}/{Year}/{Category}' (FolderId: '{FolderId}') with FileId '{FileId}'",
            fileName, rootFolderName, year, category, targetFolderId, uploadedFile.Id);

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

    private async Task<string> GetOrCreateFolderAsync(
        DriveService service,
        string folderName,
        string? parentFolderId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{parentFolderId ?? "root"}::{folderName}";
        if (_folderIdCache.TryGetValue(cacheKey, out var cachedId))
        {
            return cachedId;
        }

        var escapedName = folderName.Replace("'", "\\'");
        var query = $"mimeType = 'application/vnd.google-apps.folder' and name = '{escapedName}' and trashed = false";
        if (!string.IsNullOrWhiteSpace(parentFolderId))
        {
            query += $" and '{parentFolderId}' in parents";
        }

        var listRequest = service.Files.List();
        listRequest.Q = query;
        listRequest.Fields = "files(id, name)";
        listRequest.Spaces = "drive";

        var response = await listRequest.ExecuteAsync(cancellationToken);
        var existing = response.Files?.FirstOrDefault();
        if (existing is not null)
        {
            _folderIdCache[cacheKey] = existing.Id;
            return existing.Id;
        }

        // Folder not found; create it
        var newFolder = new Google.Apis.Drive.v3.Data.File
        {
            Name = folderName,
            MimeType = "application/vnd.google-apps.folder"
        };

        if (!string.IsNullOrWhiteSpace(parentFolderId))
        {
            newFolder.Parents = new List<string> { parentFolderId };
        }

        var createRequest = service.Files.Create(newFolder);
        createRequest.Fields = "id, name";

        var created = await createRequest.ExecuteAsync(cancellationToken);
        _logger.LogInformation("Created Google Drive folder '{Name}' under parent '{Parent}' with ID '{Id}'",
            folderName, parentFolderId ?? "root", created.Id);

        _folderIdCache[cacheKey] = created.Id;
        return created.Id;
    }

    private async Task<GoogleDriveFileReference> SaveLocallyAsync(
        string fileName,
        Stream stream,
        ReceiptUploadMetadata? metadata,
        CancellationToken cancellationToken)
    {
        var year = (metadata?.Year ?? DateTime.UtcNow.Year).ToString();
        var category = SanitizeFolderName(metadata?.Category);

        var localDir = Path.Combine(
            AppContext.BaseDirectory,
            _options.LocalStorageFallbackDirectory,
            year,
            category);
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

    public static string SanitizeFolderName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Other";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(name.Where(c => !invalidChars.Contains(c) && c != '/' && c != '\\').ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "Other" : cleaned;
    }
}
