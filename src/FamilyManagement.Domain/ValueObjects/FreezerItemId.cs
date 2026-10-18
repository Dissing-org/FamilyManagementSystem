namespace FamilyManagement.Domain.ValueObjects;

public sealed record FreezerItemId
{
    public Guid Value { get; }

    public FreezerItemId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer item ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static FreezerItemId New() => new(Guid.NewGuid());
    public static FreezerItemId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
