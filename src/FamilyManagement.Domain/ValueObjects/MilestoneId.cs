namespace FamilyManagement.Domain.ValueObjects;

public record MilestoneId(Guid Value)
{
    public static MilestoneId New() => new(Guid.NewGuid());
    public static MilestoneId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
