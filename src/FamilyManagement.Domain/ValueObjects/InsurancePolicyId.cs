namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for an insurance policy.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public record InsurancePolicyId(Guid Value)
{
    /// <summary>
    /// Creates a new <see cref="InsurancePolicyId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="InsurancePolicyId"/> instance.</returns>
    public static InsurancePolicyId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates an <see cref="InsurancePolicyId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>An <see cref="InsurancePolicyId"/> instance.</returns>
    public static InsurancePolicyId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Implicitly converts an <see cref="InsurancePolicyId"/> to its underlying <see cref="Guid"/> value.
    /// </summary>
    /// <param name="id">The insurance policy identifier.</param>
    public static implicit operator Guid(InsurancePolicyId id) => id.Value;

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> value to an <see cref="InsurancePolicyId"/>.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    public static implicit operator InsurancePolicyId(Guid value) => new(value);
}
