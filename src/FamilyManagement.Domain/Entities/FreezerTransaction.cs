using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class FreezerTransaction
{
    public FreezerTransactionId Id { get; private set; }
    public FreezerItemId ItemId { get; private set; }
    public decimal QuantityChanged { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public ConsumptionReason Reason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime Timestamp { get; private set; }

    private FreezerTransaction()
    {
        Id = null!;
        ItemId = null!;
    }

    private FreezerTransaction(
        FreezerTransactionId id,
        FreezerItemId itemId,
        decimal quantityChanged,
        decimal remainingQuantity,
        ConsumptionReason reason,
        string? notes,
        DateTime timestamp)
    {
        Id = id;
        ItemId = itemId;
        QuantityChanged = quantityChanged;
        RemainingQuantity = remainingQuantity;
        Reason = reason;
        Notes = notes;
        Timestamp = timestamp;
    }

    public static FreezerTransaction Create(
        FreezerItemId itemId,
        decimal quantityChanged,
        decimal remainingQuantity,
        ConsumptionReason reason,
        string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(itemId);

        return new FreezerTransaction(
            FreezerTransactionId.New(),
            itemId,
            quantityChanged,
            remainingQuantity,
            reason,
            notes?.Trim(),
            DateTime.UtcNow);
    }
}
