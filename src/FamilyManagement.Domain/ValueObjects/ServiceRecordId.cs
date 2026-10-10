namespace FamilyManagement.Domain.ValueObjects;

public sealed record ServiceRecordId
{
    public Guid Value { get; }

    public ServiceRecordId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Service record ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static ServiceRecordId New() => new(Guid.NewGuid());
    public static ServiceRecordId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
