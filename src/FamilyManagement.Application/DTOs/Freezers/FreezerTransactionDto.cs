using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerTransactionDto(
    Guid Id,
    Guid ItemId,
    decimal QuantityChanged,
    decimal RemainingQuantity,
    string Reason,
    string? Notes,
    DateTime Timestamp)
{
    public static FreezerTransactionDto FromDomain(FreezerTransaction tx) =>
        new(
            tx.Id.Value,
            tx.ItemId.Value,
            tx.QuantityChanged,
            tx.RemainingQuantity,
            tx.Reason.ToString(),
            tx.Notes,
            tx.Timestamp);
}
