namespace FamilyManagement.Domain.ValueObjects;

public sealed record MileageLogId
{
    public Guid Value { get; }

    public MileageLogId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Mileage log ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static MileageLogId New() => new(Guid.NewGuid());
    public static MileageLogId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
