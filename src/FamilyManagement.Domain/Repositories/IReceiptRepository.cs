using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="Receipt"/> entities.
/// </summary>
public interface IReceiptRepository
{
    /// <summary>
    /// Retrieves a receipt by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching receipt or <c>null</c> if not found.</returns>
    Task<Receipt?> GetByIdAsync(ReceiptId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves receipts matching an optional merchant name filter.
    /// </summary>
    /// <param name="merchantFilter">An optional filter for the merchant name.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of matching receipts.</returns>
    Task<IReadOnlyList<Receipt>> ListAsync(string? merchantFilter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a distinct list of all merchant names from stored receipts.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of unique merchant names.</returns>
    Task<IReadOnlyList<string>> GetDistinctMerchantsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new receipt to the repository.
    /// </summary>
    /// <param name="receipt">The receipt entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing receipt in the repository.
    /// </summary>
    /// <param name="receipt">The receipt entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this repository context to the underlying data store.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
