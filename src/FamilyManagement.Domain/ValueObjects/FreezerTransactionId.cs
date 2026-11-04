namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a freezer transaction.
/// </summary>
public sealed record FreezerTransactionId
{
    /// <summary>
    /// Gets the underlying GUID value of the identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreezerTransactionId"/> record with the specified GUID value.
    /// </summary>
    /// <param name="value">The unique identifier value.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public FreezerTransactionId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Freezer transaction ID cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="FreezerTransactionId"/> with a newly generated GUID.
    /// </summary>
    /// <returns>A new <see cref="FreezerTransactionId"/> instance.</returns>
    public static FreezerTransactionId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a <see cref="FreezerTransactionId"/> from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <returns>A <see cref="FreezerTransactionId"/> instance containing the provided value.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.</exception>
    public static FreezerTransactionId From(Guid value) => new(value);

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the underlying GUID.</returns>
    public override string ToString() => Value.ToString();
}
