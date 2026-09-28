using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Commands;

public record UploadReceiptCommand(
    string Merchant,
    DateTime PurchaseDate,
    decimal Amount,
    string Currency,
    string FileName,
    Stream FileContent,
    string ContentType,
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
        // 1. Upload file to Google Drive
        var fileReference = await _storageService.UploadAsync(
            command.FileName,
            command.FileContent,
            command.ContentType,
            cancellationToken);

        // 2. Create Domain Aggregate Root
        var money = Money.Create(command.Amount, command.Currency);
        var receipt = Receipt.Create(
            command.Merchant,
            command.PurchaseDate,
            money,
            fileReference,
            command.Notes);

        // 3. Persist to repository
        await _repository.AddAsync(receipt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return ReceiptResponseDto.FromEntity(receipt);
    }
}
