namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a freezer catalog item.
/// </summary>
public sealed record FreezerCatalogItemId
{
    /// <summary>
    /// Gets the underlying GUID value of the identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreezerCatalogItemId"/> record with the specified GUID value.
    /// </summary>
    /// <param name="value">The unique identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public FreezerCatalogItemId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer catalog item ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="FreezerCatalogItemId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="FreezerCatalogItemId"/> instance.</returns>
    public static FreezerCatalogItemId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="FreezerCatalogItemId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="FreezerCatalogItemId"/> instance containing the provided value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public static FreezerCatalogItemId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
