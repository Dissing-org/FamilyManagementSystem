using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="Vehicle"/> entities.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Retrieves a vehicle by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the vehicle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching vehicle or <c>null</c> if not found.</returns>
    Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all vehicles.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of all vehicles.</returns>
    Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new vehicle to the repository.
    /// </summary>
    /// <param name="vehicle">The vehicle entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing vehicle in the repository.
    /// </summary>
    /// <param name="vehicle">The vehicle entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a vehicle from the repository.
    /// </summary>
    /// <param name="vehicle">The vehicle entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
}
