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

    /// <summary>
    /// Creates a new <see cref="FreezerTransaction"/> recording an inventory consumption or adjustment event.
    /// </summary>
    /// <param name="itemId">The identifier of the associated freezer item.</param>
    /// <param name="quantityChanged">The quantity consumed or deducted.</param>
    /// <param name="remainingQuantity">The remaining inventory quantity after the deduction.</param>
    /// <param name="reason">The recorded reason for consumption.</param>
    /// <param name="notes">Optional notes or remarks regarding the transaction.</param>
    /// <returns>A new <see cref="FreezerTransaction"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemId"/> is null.</exception>
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
