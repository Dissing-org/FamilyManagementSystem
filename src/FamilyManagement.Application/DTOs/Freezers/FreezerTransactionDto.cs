using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing a transaction log entry for a freezer item consumption or modification.
/// </summary>
/// <param name="Id">The unique identifier of the transaction record.</param>
/// <param name="ItemId">The unique identifier of the freezer item.</param>
/// <param name="QuantityChanged">The quantity change applied during this transaction.</param>
/// <param name="RemainingQuantity">The quantity remaining after this transaction occurred.</param>
/// <param name="Reason">The consumption reason description.</param>
/// <param name="Notes">Optional notes associated with the transaction.</param>
/// <param name="Timestamp">The timestamp when the transaction occurred.</param>
public record FreezerTransactionDto(
    Guid Id,
    Guid ItemId,
    decimal QuantityChanged,
    decimal RemainingQuantity,
    string Reason,
    string? Notes,
    DateTime Timestamp)
{
    /// <summary>
    /// Creates a new <see cref="FreezerTransactionDto"/> from a domain <see cref="FreezerTransaction"/> entity.
    /// </summary>
    /// <param name="tx">The domain freezer transaction entity.</param>
    /// <returns>A <see cref="FreezerTransactionDto"/> populated with values from the domain transaction entity.</returns>
    public static FreezerTransactionDto FromDomain(FreezerTransaction tx) =>
        new(
            tx.Id.Value,
            tx.ItemId.Value,
            tx.QuantityChanged,
            tx.RemainingQuantity,
            tx.Reason.ToString(),
            tx.Notes,
            tx.Timestamp);
}
