using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Commands;

public record UploadReceiptCommand(
    string Merchant,
    DateTime PurchaseDate,
    string FileName,
    Stream FileContent,
    string ContentType,
    string? Category = null,
    string? Notes = null);

public class UploadReceiptCommandHandler
{
    private readonly IReceiptRepository _repository;
    private readonly IReceiptFileStorageService _storageService;

    public UploadReceiptCommandHandler(
        IReceiptRepository repository,
        IReceiptFileStorageService storageService)
    {
        _repository = repository;
        _storageService = storageService;
    }

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
