namespace FamilyManagement.Domain.ValueObjects;

public sealed record FreezerCatalogItemId
{
    public Guid Value { get; }

    public FreezerCatalogItemId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer catalog item ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static FreezerCatalogItemId New() => new(Guid.NewGuid());
    public static FreezerCatalogItemId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
