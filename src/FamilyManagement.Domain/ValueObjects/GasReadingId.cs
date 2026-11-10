namespace FamilyManagement.Domain.ValueObjects;

public sealed record GasReadingId
{
    public Guid Value { get; }

    public GasReadingId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Gas reading ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static GasReadingId New() => new(Guid.NewGuid());
    public static GasReadingId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
