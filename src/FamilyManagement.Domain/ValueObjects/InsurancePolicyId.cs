namespace FamilyManagement.Domain.ValueObjects;

public record InsurancePolicyId(Guid Value)
{
    public static InsurancePolicyId New() => new(Guid.NewGuid());
    public static InsurancePolicyId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(InsurancePolicyId id) => id.Value;
    public static implicit operator InsurancePolicyId(Guid value) => new(value);
}
