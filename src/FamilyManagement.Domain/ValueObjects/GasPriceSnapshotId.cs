namespace FamilyManagement.Domain.ValueObjects;

public sealed record GasPriceSnapshotId
{
    public Guid Value { get; }

    public GasPriceSnapshotId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Gas price snapshot ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static GasPriceSnapshotId New() => new(Guid.NewGuid());
    public static GasPriceSnapshotId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
