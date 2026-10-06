namespace FamilyManagement.Domain.ValueObjects;

public record GrowthMeasurementId(Guid Value)
{
    public static GrowthMeasurementId New() => new(Guid.NewGuid());
    public static GrowthMeasurementId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
