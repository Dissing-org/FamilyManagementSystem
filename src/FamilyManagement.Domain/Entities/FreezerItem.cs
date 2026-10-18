using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Services;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class FreezerItem
{
    public FreezerItemId Id { get; private set; }
    public FreezerCompartmentId CompartmentId { get; private set; }
    public string Name { get; private set; }
    public FreezerCategory Category { get; private set; }
    public decimal Quantity { get; private set; }
    public FreezerUnit Unit { get; private set; }
    public DateTime FreezeDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public decimal? MinimumQuantity { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private FreezerItem()
    {
        Id = null!;
        CompartmentId = null!;
        Name = null!;
    }

    private FreezerItem(
        FreezerItemId id,
        FreezerCompartmentId compartmentId,
        string name,
        FreezerCategory category,
        decimal quantity,
        FreezerUnit unit,
        DateTime freezeDate,
        DateTime? expiryDate,
        decimal? minimumQuantity,
        string? notes,
        DateTime createdAt)
    {
        Id = id;
        CompartmentId = compartmentId;
        Name = name;
        Category = category;
        Quantity = quantity;
        Unit = unit;
        FreezeDate = DateTime.SpecifyKind(freezeDate, DateTimeKind.Utc);
        ExpiryDate = expiryDate.HasValue ? DateTime.SpecifyKind(expiryDate.Value, DateTimeKind.Utc) : null;
        MinimumQuantity = minimumQuantity;
        Notes = notes;
        CreatedAt = createdAt;
    }

    public static FreezerItem Create(
        string name,
        FreezerCompartmentId compartmentId,
        FreezerCategory category,
        decimal quantity,
        FreezerUnit unit,
        DateTime freezeDate,
        DateTime? expiryDate = null,
        decimal? minimumQuantity = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Item name is required.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(compartmentId);

        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        }

        if (minimumQuantity.HasValue && minimumQuantity.Value < 0)
        {
            throw new ArgumentException("Minimum quantity cannot be negative.", nameof(minimumQuantity));
        }

        var effectiveExpiry = expiryDate ?? FreezerShelfLifeRules.CalculateDefaultExpiry(category, freezeDate);

        return new FreezerItem(
            FreezerItemId.New(),
            compartmentId,
            name.Trim(),
            category,
            quantity,
            unit,
            freezeDate,
            effectiveExpiry,
            minimumQuantity,
            notes?.Trim(),
            DateTime.UtcNow);
    }

    public void UpdateDetails(
        string name,
        FreezerCompartmentId compartmentId,
        FreezerCategory category,
        decimal quantity,
        FreezerUnit unit,
        DateTime freezeDate,
        DateTime? expiryDate,
        decimal? minimumQuantity,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Item name is required.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(compartmentId);

        if (quantity < 0)
        {
            throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
        }

        if (minimumQuantity.HasValue && minimumQuantity.Value < 0)
        {
            throw new ArgumentException("Minimum quantity cannot be negative.", nameof(minimumQuantity));
        }

        Name = name.Trim();
        CompartmentId = compartmentId;
        Category = category;
        Quantity = quantity;
        Unit = unit;
        FreezeDate = DateTime.SpecifyKind(freezeDate, DateTimeKind.Utc);
        ExpiryDate = expiryDate.HasValue ? DateTime.SpecifyKind(expiryDate.Value, DateTimeKind.Utc) : null;
        MinimumQuantity = minimumQuantity;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public FreezerTransaction Consume(decimal amount, ConsumptionReason reason, string? notes = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount consumed must be greater than zero.", nameof(amount));
        }

        if (amount > Quantity)
        {
            throw new InvalidOperationException($"Cannot consume {amount} when only {Quantity} is available.");
        }

        Quantity -= amount;
        UpdatedAt = DateTime.UtcNow;

        return FreezerTransaction.Create(Id, amount, Quantity, reason, notes);
    }

    public void AddQuantity(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount added must be greater than zero.", nameof(amount));
        }

        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveToCompartment(FreezerCompartmentId newCompartmentId)
    {
        ArgumentNullException.ThrowIfNull(newCompartmentId);
        CompartmentId = newCompartmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public FreshnessStatus EvaluateFreshness(DateTime referenceDate)
    {
        if (!ExpiryDate.HasValue)
        {
            return FreshnessStatus.Fresh;
        }

        if (referenceDate.Date > ExpiryDate.Value.Date)
        {
            return FreshnessStatus.Expired;
        }

        if (ExpiryDate.Value.Date - referenceDate.Date <= TimeSpan.FromDays(14))
        {
            return FreshnessStatus.ExpiringSoon;
        }

        return FreshnessStatus.Fresh;
    }

    public bool IsLowStock()
    {
        return MinimumQuantity.HasValue && Quantity <= MinimumQuantity.Value;
    }
}
