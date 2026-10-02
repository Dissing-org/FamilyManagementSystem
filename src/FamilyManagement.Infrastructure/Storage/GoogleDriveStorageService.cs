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
    private readonly IGoogleDriveAuthService? _authService;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _folderIdCache = new();

    public GoogleDriveStorageService(
        IOptions<GoogleDriveOptions> options,
        ILogger<GoogleDriveStorageService> logger,
        IGoogleDriveAuthService? authService = null)
    {
        _options = options.Value;
        _logger = logger;
        _authService = authService;
    }

    private async Task<DriveService?> TryGetDriveServiceAsync(CancellationToken cancellationToken)
    {
        // 1. Try OAuth 2.0 User Credential (preferred for personal Google Drive)
        if (_authService is GoogleDriveAuthService concreteAuth)
        {
            var userDrive = await concreteAuth.TryCreateUserDriveServiceAsync(cancellationToken);
            if (userDrive is not null)
            {
                return userDrive;
            }
        }

        // 2. Try Service Account (for Workspace Shared Drives)
        if (!string.IsNullOrWhiteSpace(_options.CredentialsJsonPath) && File.Exists(_options.CredentialsJsonPath))
        {
            var credential = GoogleCredential.FromFile(_options.CredentialsJsonPath)
                .CreateScoped(DriveService.Scope.DriveFile);

            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "FamilyManagementSystem"
            });
        }

        return null;
    }

    public Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        return UploadAsync(fileName, fileStream, contentType, metadata: null, cancellationToken);
    }

    public string ResolveLocalStorageDirectory()
    {
        var configured = _options.LocalStorageDirectory;
        if (string.IsNullOrWhiteSpace(configured))
        {
            configured = _options.LocalStorageFallbackDirectory;
        }

        // If running in a Linux container but configured with a Windows-style path (e.g. C:\... or C:/...),
        // fall back to /app/receipts_storage if it exists
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) &&
            !string.IsNullOrWhiteSpace(configured) &&
            (configured.Length > 1 && configured[1] == ':'))
        {
            if (Directory.Exists("/app/receipts_storage"))
            {
                return "/app/receipts_storage";
            }
            configured = _options.LocalStorageFallbackDirectory ?? "receipts_storage";
        }

        if (string.IsNullOrWhiteSpace(configured))
        {
            configured = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "FamilyManagementReceipts");
        }

        return Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(AppContext.BaseDirectory, configured);
    }

    public async Task<GoogleDriveFileReference> UploadAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        ReceiptUploadMetadata? metadata,
        CancellationToken cancellationToken = default)
    {
        // If configured for Local storage, save directly to local PC directory
        if (string.Equals(_options.StorageProvider, "Local", StringComparison.OrdinalIgnoreCase))
        {
            return await SaveLocallyAsync(fileName, fileStream, metadata, cancellationToken);
        }

        var driveService = await TryGetDriveServiceAsync(cancellationToken);
        if (driveService is null)
        {
            if (_options.UseLocalStorageFallbackWhenUnconfigured)
            {
                _logger.LogWarning("Google Drive is not configured or not authorized. Using local storage directory '{Dir}'", ResolveLocalStorageDirectory());
                return await SaveLocallyAsync(fileName, fileStream, metadata, cancellationToken);
            }

            throw new InvalidOperationException("Google Drive is not authorized or configured.");
        }

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
        // Try deleting local file first if present
        var baseDir = ResolveLocalStorageDirectory();
        if (Directory.Exists(baseDir))
        {
            var matchedFiles = Directory.GetFiles(baseDir, $"{fileId}_*", SearchOption.AllDirectories);
            foreach (var file in matchedFiles)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted local receipt file '{Path}'", file);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete local receipt file '{Path}'", file);
                }
            }

            if (string.Equals(_options.StorageProvider, "Local", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        var driveService = await TryGetDriveServiceAsync(cancellationToken);
        if (driveService is not null)
        {
            try
            {
                await driveService.Files.Delete(fileId).ExecuteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete Google Drive file '{FileId}'", fileId);
            }
        }
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

        var baseDir = ResolveLocalStorageDirectory();
        var localDir = Path.Combine(baseDir, year, category);
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

        _logger.LogInformation("Saved receipt '{FileName}' to local disk at '{Path}'", fileName, destinationPath);

        var webLink = $"/api/receipts/files/{fileId}";
        return GoogleDriveFileReference.Create(fileId, savedName, webLink);
    }

    private static readonly HashSet<char> DisallowedChars = new(
        Path.GetInvalidFileNameChars()
            .Concat(new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|', '\0' }));

    public static string SanitizeFolderName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Other";
        }

        var cleaned = new string(name.Where(c => !DisallowedChars.Contains(c)).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "Other" : cleaned;
    }
}
