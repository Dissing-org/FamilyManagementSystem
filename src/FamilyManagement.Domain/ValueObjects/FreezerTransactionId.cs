namespace FamilyManagement.Domain.ValueObjects;

public sealed record FreezerTransactionId
{
    public Guid Value { get; }

    public FreezerTransactionId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer transaction ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static FreezerTransactionId New() => new(Guid.NewGuid());
    public static FreezerTransactionId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
