namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a developmental milestone.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public record MilestoneId(Guid Value)
{
    /// <summary>
    /// Creates a new <see cref="MilestoneId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="MilestoneId"/> instance.</returns>
    public static MilestoneId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="MilestoneId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="MilestoneId"/> instance containing the provided value.</returns>
    public static MilestoneId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
