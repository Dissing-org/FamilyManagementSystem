namespace FamilyManagement.Domain.ValueObjects;

public sealed record FreezerCompartmentId
{
    public Guid Value { get; }

    public FreezerCompartmentId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer compartment ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public static FreezerCompartmentId New() => new(Guid.NewGuid());
    public static FreezerCompartmentId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
