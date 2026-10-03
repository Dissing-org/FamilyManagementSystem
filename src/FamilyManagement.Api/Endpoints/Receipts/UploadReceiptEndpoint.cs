using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Commands;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class UploadReceiptRequest
{
    public IFormFile File { get; set; } = null!;
    public string Merchant { get; set; } = string.Empty;
    public DateTime? PurchaseDate { get; set; }
    public string? Category { get; set; } = "Other";
    public string? Notes { get; set; }
}

public class UploadReceiptEndpoint : Endpoint<UploadReceiptRequest, ReceiptResponseDto>
{
    private readonly UploadReceiptCommandHandler _handler;

    public UploadReceiptEndpoint(UploadReceiptCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/receipts");
        AllowAnonymous();
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Upload and archive a purchase receipt";
            s.Description = "Uploads a receipt image/PDF to Google Drive, tracks metadata in SQLite, and returns the receipt record.";
        });
    }

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
