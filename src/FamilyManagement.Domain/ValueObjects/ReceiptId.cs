namespace FamilyManagement.Domain.ValueObjects;

public sealed record ReceiptId
{
    public Guid Value { get; }

    public ReceiptId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Receipt ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static ReceiptId New() => new(Guid.NewGuid());
    public static ReceiptId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
