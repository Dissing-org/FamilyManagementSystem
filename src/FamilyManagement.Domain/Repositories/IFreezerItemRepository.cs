using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="FreezerItem"/> entities and associated transactions.
/// </summary>
public interface IFreezerItemRepository
{
    /// <summary>
    /// Retrieves all freezer items.
    /// </summary>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of all freezer items.</returns>
    Task<IReadOnlyList<FreezerItem>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves all freezer items located in a specific compartment.
    /// </summary>
    /// <param name="compartmentId">The unique identifier of the compartment.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of items in the compartment.</returns>
    Task<IReadOnlyList<FreezerItem>> GetByCompartmentIdAsync(FreezerCompartmentId compartmentId, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a freezer item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the freezer item.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching freezer item or <c>null</c> if not found.</returns>
    Task<FreezerItem?> GetByIdAsync(FreezerItemId id, CancellationToken ct = default);

    /// <summary>
    /// Persists a new freezer item to the repository.
    /// </summary>
    /// <param name="item">The freezer item entity to add.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(FreezerItem item, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing freezer item in the repository.
    /// </summary>
    /// <param name="item">The freezer item entity with updated values.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(FreezerItem item, CancellationToken ct = default);

    /// <summary>
    /// Deletes a freezer item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the freezer item to delete.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(FreezerItemId id, CancellationToken ct = default);

    /// <summary>
    /// Persists a new freezer inventory transaction to the repository.
    /// </summary>
    /// <param name="transaction">The transaction entity to add.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddTransactionAsync(FreezerTransaction transaction, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all inventory transactions recorded for a specific freezer item.
    /// </summary>
    /// <param name="itemId">The unique identifier of the freezer item.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of transaction records.</returns>
    Task<IReadOnlyList<FreezerTransaction>> GetTransactionsByItemIdAsync(FreezerItemId itemId, CancellationToken ct = default);
}
