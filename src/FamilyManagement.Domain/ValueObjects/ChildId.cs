namespace FamilyManagement.Domain.ValueObjects;

public record ChildId(Guid Value)
{
    public static ChildId New() => new(Guid.NewGuid());
    public static ChildId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
