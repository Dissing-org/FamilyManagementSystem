namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a growth measurement.
/// </summary>
/// <param name="Value">The underlying GUID value.</param>
public record GrowthMeasurementId(Guid Value)
{
    /// <summary>
    /// Creates a new <see cref="GrowthMeasurementId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="GrowthMeasurementId"/> instance.</returns>
    public static GrowthMeasurementId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="GrowthMeasurementId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="GrowthMeasurementId"/> instance containing the provided value.</returns>
    public static GrowthMeasurementId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
