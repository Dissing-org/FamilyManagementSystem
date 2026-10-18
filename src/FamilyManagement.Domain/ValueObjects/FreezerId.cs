namespace FamilyManagement.Domain.ValueObjects;

public sealed record FreezerId
{
    public Guid Value { get; }

    public FreezerId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static FreezerId New() => new(Guid.NewGuid());
    public static FreezerId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
