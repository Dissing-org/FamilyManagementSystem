using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="Freezer"/> entities and their compartments.
/// </summary>
public interface IFreezerRepository
{
    /// <summary>
    /// Retrieves all freezers along with their compartments.
    /// </summary>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of freezers including compartments.</returns>
    Task<IReadOnlyList<Freezer>> GetAllWithCompartmentsAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves a freezer by its unique identifier, including its compartments.
    /// </summary>
    /// <param name="id">The unique identifier of the freezer.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching freezer or <c>null</c> if not found.</returns>
    Task<Freezer?> GetByIdAsync(FreezerId id, CancellationToken ct = default);

    /// <summary>
    /// Persists a new freezer to the repository.
    /// </summary>
    /// <param name="freezer">The freezer entity to add.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Freezer freezer, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing freezer in the repository.
    /// </summary>
    /// <param name="freezer">The freezer entity with updated values.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Freezer freezer, CancellationToken ct = default);

    /// <summary>
    /// Deletes a freezer by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the freezer to delete.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(FreezerId id, CancellationToken ct = default);
}
