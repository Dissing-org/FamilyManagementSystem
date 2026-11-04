using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class FreezerCatalogItem
{
    public FreezerCatalogItemId Id { get; private set; }
    public string Name { get; private set; }
    public string NormalizedName { get; private set; }
    public FreezerCategory DefaultCategory { get; private set; }
    public FreezerUnit DefaultUnit { get; private set; }
    public decimal? DefaultMinimumQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }

    private FreezerCatalogItem()
    {
        Id = null!;
        Name = null!;
        NormalizedName = null!;
    }

    private FreezerCatalogItem(
        FreezerCatalogItemId id,
        string name,
        string normalizedName,
        FreezerCategory defaultCategory,
        FreezerUnit defaultUnit,
        decimal? defaultMinimumQuantity,
        DateTime createdAt,
        DateTime lastUsedAt)
    {
        Id = id;
        Name = name;
        NormalizedName = normalizedName;
        DefaultCategory = defaultCategory;
        DefaultUnit = defaultUnit;
        DefaultMinimumQuantity = defaultMinimumQuantity;
        CreatedAt = DateTime.SpecifyKind(createdAt, DateTimeKind.Utc);
        LastUsedAt = DateTime.SpecifyKind(lastUsedAt, DateTimeKind.Utc);
    }

    /// <summary>
    /// Creates a new <see cref="FreezerCatalogItem"/> for autocompleting and templating frequently stored freezer items.
    /// </summary>
    /// <param name="name">The name of the catalog item.</param>
    /// <param name="defaultCategory">The default category suggested for this item.</param>
    /// <param name="defaultUnit">The default measurement unit suggested for this item.</param>
    /// <param name="defaultMinimumQuantity">The optional default minimum threshold for low-stock warnings.</param>
    /// <returns>A new <see cref="FreezerCatalogItem"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, whitespace, or exceeds 150 characters, or when <paramref name="defaultMinimumQuantity"/> is negative.</exception>
    public static FreezerCatalogItem Create(
        string name,
        FreezerCategory defaultCategory,
        FreezerUnit defaultUnit,
        decimal? defaultMinimumQuantity = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Catalog item name is required.", nameof(name));
        }

        var trimmed = name.Trim();
        if (trimmed.Length > 150)
        {
            throw new ArgumentException("Catalog item name cannot exceed 150 characters.", nameof(name));
        }

        if (defaultMinimumQuantity.HasValue && defaultMinimumQuantity.Value < 0)
        {
            throw new ArgumentException("Default minimum quantity cannot be negative.", nameof(defaultMinimumQuantity));
        }

        var now = DateTime.UtcNow;
        return new FreezerCatalogItem(
            FreezerCatalogItemId.New(),
            trimmed,
            trimmed.ToUpperInvariant(),
            defaultCategory,
            defaultUnit,
            defaultMinimumQuantity,
            now,
            now);
    }

    /// <summary>
    /// Updates the default catalog categorization, unit of measure, and optional minimum stock quantity.
    /// </summary>
    /// <param name="category">The updated default category.</param>
    /// <param name="unit">The updated default measurement unit.</param>
    /// <param name="minimumQuantity">The updated optional default minimum quantity threshold.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="minimumQuantity"/> is negative.</exception>
    public void UpdateDefaults(FreezerCategory category, FreezerUnit unit, decimal? minimumQuantity = null)
    {
        if (minimumQuantity.HasValue && minimumQuantity.Value < 0)
        {
            throw new ArgumentException("Default minimum quantity cannot be negative.", nameof(minimumQuantity));
        }

        DefaultCategory = category;
        DefaultUnit = unit;
        DefaultMinimumQuantity = minimumQuantity ?? DefaultMinimumQuantity;
        LastUsedAt = DateTime.UtcNow;
    }
}
