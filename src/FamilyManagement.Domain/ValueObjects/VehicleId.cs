namespace FamilyManagement.Domain.ValueObjects;

public sealed record VehicleId
{
    public Guid Value { get; }

    public VehicleId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Vehicle ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static VehicleId New() => new(Guid.NewGuid());
    public static VehicleId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
