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

    /// <summary>
    /// Creates a new <see cref="FreezerItem"/> instance with inventory tracking and shelf life calculation.
    /// </summary>
    /// <param name="name">The name or description of the food item.</param>
    /// <param name="compartmentId">The identifier of the compartment storing this item.</param>
    /// <param name="category">The food category for the item.</param>
    /// <param name="quantity">The initial quantity stored.</param>
    /// <param name="unit">The unit of measure for the quantity.</param>
    /// <param name="freezeDate">The date when the item was frozen.</param>
    /// <param name="expiryDate">The optional explicit expiration date. When null, a default is calculated based on category shelf life.</param>
    /// <param name="minimumQuantity">The optional minimum quantity threshold for low stock alerts.</param>
    /// <param name="notes">Optional notes or remarks regarding the item.</param>
    /// <returns>A new <see cref="FreezerItem"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="compartmentId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace, or when <paramref name="quantity"/> or <paramref name="minimumQuantity"/> is negative.</exception>
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

    /// <summary>
    /// Updates all configurable properties of the freezer item.
    /// </summary>
    /// <param name="name">The updated name of the item.</param>
    /// <param name="compartmentId">The updated compartment identifier.</param>
    /// <param name="category">The updated category.</param>
    /// <param name="quantity">The updated quantity.</param>
    /// <param name="unit">The updated unit of measure.</param>
    /// <param name="freezeDate">The updated freeze date.</param>
    /// <param name="expiryDate">The updated optional expiry date.</param>
    /// <param name="minimumQuantity">The updated optional minimum quantity threshold.</param>
    /// <param name="notes">Updated optional notes.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="compartmentId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace, or when <paramref name="quantity"/> or <paramref name="minimumQuantity"/> is negative.</exception>
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

    /// <summary>
    /// Consumes a specified quantity of the item, decreasing inventory and producing a transaction record.
    /// </summary>
    /// <param name="amount">The quantity to consume.</param>
    /// <param name="reason">The reason for consumption or disposal.</param>
    /// <param name="notes">Optional notes regarding the transaction.</param>
    /// <returns>A new <see cref="FreezerTransaction"/> documenting the change.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="amount"/> is less than or equal to zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="amount"/> exceeds the currently available <see cref="Quantity"/>.</exception>
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

    /// <summary>
    /// Adds more quantity to the existing inventory of this item.
    /// </summary>
    /// <param name="amount">The quantity amount to add.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="amount"/> is less than or equal to zero.</exception>
    public void AddQuantity(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount added must be greater than zero.", nameof(amount));
        }

        Quantity += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Moves the item to a different freezer compartment.
    /// </summary>
    /// <param name="newCompartmentId">The identifier of the destination compartment.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="newCompartmentId"/> is null.</exception>
    public void MoveToCompartment(FreezerCompartmentId newCompartmentId)
    {
        ArgumentNullException.ThrowIfNull(newCompartmentId);
        CompartmentId = newCompartmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Evaluates the freshness status of the item relative to a specified reference date.
    /// </summary>
    /// <param name="referenceDate">The reference date to evaluate freshness against (typically current date).</param>
    /// <returns>A <see cref="FreshnessStatus"/> value representing whether the item is fresh, expiring soon, or expired.</returns>
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

    /// <summary>
    /// Determines whether the item's quantity is at or below its configured minimum stock threshold.
    /// </summary>
    /// <returns><c>true</c> if minimum quantity is configured and current quantity is less than or equal to it; otherwise, <c>false</c>.</returns>
    public bool IsLowStock()
    {
        return MinimumQuantity.HasValue && Quantity <= MinimumQuantity.Value;
    }
}
