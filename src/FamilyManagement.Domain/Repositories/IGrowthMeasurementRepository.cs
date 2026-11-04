using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="GrowthMeasurement"/> entities.
/// </summary>
public interface IGrowthMeasurementRepository
{
    /// <summary>
    /// Persists a new growth measurement to the repository.
    /// </summary>
    /// <param name="measurement">The growth measurement entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a growth measurement by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the growth measurement.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching growth measurement or <c>null</c> if not found.</returns>
    Task<GrowthMeasurement?> GetByIdAsync(GrowthMeasurementId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all growth measurements recorded for a specific child.
    /// </summary>
    /// <param name="childId">The unique identifier of the child.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of growth measurements for the child.</returns>
    Task<List<GrowthMeasurement>> GetByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the most recent growth measurement recorded for a specific child.
    /// </summary>
    /// <param name="childId">The unique identifier of the child.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the latest growth measurement or <c>null</c> if none exist.</returns>
    Task<GrowthMeasurement?> GetLatestByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing growth measurement in the repository.
    /// </summary>
    /// <param name="measurement">The growth measurement entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a growth measurement from the repository.
    /// </summary>
    /// <param name="measurement">The growth measurement entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);
}
