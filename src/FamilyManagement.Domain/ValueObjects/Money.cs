namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Represents a monetary value with an associated currency code.
/// </summary>
public sealed record Money
{
    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the 3-letter ISO currency code.
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a new <see cref="Money"/> instance after validating the amount and currency code.
    /// </summary>
    /// <param name="amount">The monetary amount, which must not be negative.</param>
    /// <param name="currency">The 3-letter ISO currency code.</param>
    /// <returns>A validated <see cref="Money"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="amount"/> is negative or <paramref name="currency"/> is not a valid 3-letter code.</exception>
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            throw new ArgumentException("Currency must be a valid 3-letter ISO code.", nameof(currency));
        }

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }

    /// <summary>
    /// Returns the string representation of the monetary value formatted with its currency code.
    /// </summary>
    /// <returns>A string representation of the money value.</returns>
    public override string ToString() => $"{Amount:F2} {Currency}";
}
