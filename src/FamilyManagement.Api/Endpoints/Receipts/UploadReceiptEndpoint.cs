using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Commands;

namespace FamilyManagement.Api.Endpoints.Receipts;

/// <summary>
/// Request payload for uploading and archiving a receipt.
/// </summary>
public class UploadReceiptRequest
{
    public IFormFile File { get; set; } = null!;
    public string Merchant { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public string? Category { get; set; } = "Other";
    public string? Notes { get; set; }
}

/// <summary>
/// Endpoint for uploading, storing, and indexing purchase receipts.
/// </summary>
public class UploadReceiptEndpoint : Endpoint<UploadReceiptRequest, ReceiptResponseDto>
{
    private readonly UploadReceiptCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadReceiptEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process receipt uploads.</param>
    public UploadReceiptEndpoint(UploadReceiptCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, file upload settings, and permissions.
    /// </summary>
    public override void Configure()
    {
        Post("/api/receipts");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload and archive a purchase receipt";
            s.Description = "Uploads a receipt image/PDF to Google Drive, tracks metadata in SQLite, and returns the receipt record.";
        });
    }

    /// <summary>
    /// Handles the receipt upload request asynchronously.
    /// </summary>
    /// <param name="req">The request containing the receipt file and metadata.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(UploadReceiptRequest req, CancellationToken ct)
    {
        if (req.File is null || req.File.Length == 0)
        {
            ThrowError("A receipt file must be provided.");
        }

        using var stream = req.File.OpenReadStream();

        var command = new UploadReceiptCommand(
            Merchant: req.Merchant,
            PurchaseDate: req.PurchaseDate ?? DateTime.UtcNow,
            FileName: req.File.FileName,
            FileContent: stream,
            ContentType: req.File.ContentType ?? "application/octet-stream",
            Category: req.Category,
            Notes: req.Notes);

        try
        {
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/receipts/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            ThrowError(ex.Message);
        }
    }
}
