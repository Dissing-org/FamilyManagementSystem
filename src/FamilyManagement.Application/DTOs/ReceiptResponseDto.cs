using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Application.DTOs;

public record ReceiptResponseDto(
    Guid Id,
    string Merchant,
    DateTime PurchaseDate,
    decimal Amount,
    string Currency,
    string Category,
    string GoogleDriveFileId,
    string FileName,
    string? WebViewLink,
    string? Notes,
    ReceiptStatus Status,
    DateTime CreatedAt,
    DateTime? ArchivedAt)
{
    public static ReceiptResponseDto FromEntity(Receipt receipt) =>
        new(
            receipt.Id.Value,
            receipt.Merchant,
            receipt.PurchaseDate,
            receipt.Amount.Amount,
            receipt.Amount.Currency,
            receipt.Category,
            receipt.FileReference.FileId,
            receipt.FileReference.FileName,
            receipt.FileReference.WebViewLink,
            receipt.Notes,
            receipt.Status,
            receipt.CreatedAt,
            receipt.ArchivedAt);
}
