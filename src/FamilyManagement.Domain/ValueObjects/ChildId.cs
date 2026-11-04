namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a child profile.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public record ChildId(Guid Value)
{
    /// <summary>
    /// Creates a new <see cref="ChildId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="ChildId"/> instance.</returns>
    public static ChildId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="ChildId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="ChildId"/> instance containing the provided value.</returns>
    public static ChildId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
