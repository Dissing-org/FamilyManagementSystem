using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="ChildProfile"/> entities.
/// </summary>
public interface IChildProfileRepository
{
    /// <summary>
    /// Persists a new child profile to the repository.
    /// </summary>
    /// <param name="child">The child profile entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(ChildProfile child, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a child profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the child profile.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching child profile or <c>null</c> if not found.</returns>
    Task<ChildProfile?> GetByIdAsync(ChildId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all child profiles.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of all child profiles.</returns>
    Task<List<ChildProfile>> ListAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing child profile in the repository.
    /// </summary>
    /// <param name="child">The child profile entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(ChildProfile child, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a child profile from the repository.
    /// </summary>
    /// <param name="child">The child profile entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(ChildProfile child, CancellationToken cancellationToken = default);
}
