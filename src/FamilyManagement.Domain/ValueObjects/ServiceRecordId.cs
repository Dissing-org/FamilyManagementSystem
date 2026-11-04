namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a vehicle service record.
/// </summary>
public sealed record ServiceRecordId
{
    /// <summary>
    /// Gets the underlying GUID value of the identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRecordId"/> record with the specified GUID value.
    /// </summary>
    /// <param name="value">The unique identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public ServiceRecordId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Service record ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="ServiceRecordId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="ServiceRecordId"/> instance.</returns>
    public static ServiceRecordId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="ServiceRecordId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="ServiceRecordId"/> instance containing the provided value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public static ServiceRecordId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
