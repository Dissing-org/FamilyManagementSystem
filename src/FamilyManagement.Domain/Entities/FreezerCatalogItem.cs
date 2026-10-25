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

    public void TouchUsage()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
