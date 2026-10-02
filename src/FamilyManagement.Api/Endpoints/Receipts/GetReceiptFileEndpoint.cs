using FastEndpoints;
using Microsoft.Extensions.Options;
using FamilyManagement.Infrastructure.Storage;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class GetReceiptFileRequest
{
    public string FileId { get; set; } = string.Empty;
}

public class GetReceiptFileEndpoint : Endpoint<GetReceiptFileRequest>
{
    private readonly GoogleDriveOptions _options;

    public GetReceiptFileEndpoint(IOptions<GoogleDriveOptions> options)
    {
        _options = options.Value;
    }

    public override void Configure()
    {
        Get("/api/receipts/files/{FileId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Stream or view a locally stored receipt file";
            s.Description = "Retrieves the receipt PDF or image stored on the local PC disk.";
        });
    }

    public override async Task HandleAsync(GetReceiptFileRequest req, CancellationToken ct)
    {
        var configured = _options.LocalStorageDirectory;
        if (string.IsNullOrWhiteSpace(configured))
        {
            configured = _options.LocalStorageFallbackDirectory;
        }

        if (string.IsNullOrWhiteSpace(configured))
        {
            configured = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "FamilyManagementReceipts");
        }

        var baseDir = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(AppContext.BaseDirectory, configured);

        if (!Directory.Exists(baseDir))
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound("Storage directory not found."));
            return;
        }

        var matchingFiles = Directory.GetFiles(baseDir, $"{req.FileId}_*", SearchOption.AllDirectories);
        var filePath = matchingFiles.FirstOrDefault();

        if (filePath is null || !File.Exists(filePath))
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound("Receipt file not found on disk."));
            return;
        }

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await HttpContext.Response.SendResultAsync(TypedResults.File(stream, contentType, enableRangeProcessing: true));
    }
}
