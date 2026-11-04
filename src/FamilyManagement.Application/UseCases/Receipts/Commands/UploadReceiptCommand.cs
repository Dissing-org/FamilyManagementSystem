using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Commands;

/// <summary>
/// Command to upload a receipt file and save its metadata.
/// </summary>
/// <param name="Merchant">The merchant or vendor where the purchase occurred.</param>
/// <param name="PurchaseDate">The date of the purchase transaction.</param>
/// <param name="FileName">The file name of the receipt document or image.</param>
/// <param name="FileContent">The file data stream.</param>
/// <param name="ContentType">The MIME content type of the uploaded file.</param>
/// <param name="Category">The optional category classifying the receipt.</param>
/// <param name="Notes">Optional notes regarding the purchase.</param>
public record UploadReceiptCommand(
    string Merchant,
    DateTime PurchaseDate,
    string FileName,
    Stream FileContent,
    string ContentType,
    string? Category = null,
    string? Notes = null);

/// <summary>
/// Handler for uploading receipt files and creating receipt records.
/// </summary>
public class UploadReceiptCommandHandler
{
    private readonly IReceiptRepository _repository;
    private readonly IReceiptFileStorageService _storageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UploadReceiptCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The receipt repository.</param>
    /// <param name="storageService">The receipt file storage service.</param>
    public UploadReceiptCommandHandler(
        IReceiptRepository repository,
        IReceiptFileStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

    /// <summary>
    /// Handles uploading a receipt file to storage and persisting the receipt domain entity.
    /// </summary>
    /// <param name="command">The command containing the upload details and file stream.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="ReceiptResponseDto"/>.</returns>
    public async Task<ReceiptResponseDto> HandleAsync(UploadReceiptCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Upload file to Google Drive with folder metadata (Year/Category)
        var metadata = new ReceiptUploadMetadata(
            Category: command.Category,
            Year: command.PurchaseDate.Year,
            Merchant: command.Merchant);

        var fileReference = await _storageService.UploadAsync(
            command.FileName,
            command.FileContent,
            command.ContentType,
            metadata,
            cancellationToken);

        // 2. Create Domain Aggregate Root
        var receipt = Receipt.Create(
            command.Merchant,
            command.PurchaseDate,
            fileReference,
            command.Category,
            command.Notes);

        // 3. Persist to repository
        await _repository.AddAsync(receipt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ReceiptResponseDto.FromEntity(receipt);
    }
}
