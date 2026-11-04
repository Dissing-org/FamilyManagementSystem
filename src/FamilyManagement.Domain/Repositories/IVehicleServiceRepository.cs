using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="VehicleServiceRecord"/> entities.
/// </summary>
public interface IVehicleServiceRepository
{
    /// <summary>
    /// Retrieves all vehicle service records for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of service records for the vehicle.</returns>
    Task<List<VehicleServiceRecord>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a vehicle service record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the service record.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching service record or <c>null</c> if not found.</returns>
    Task<VehicleServiceRecord?> GetByIdAsync(ServiceRecordId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new vehicle service record to the repository.
    /// </summary>
    /// <param name="serviceRecord">The vehicle service record entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a vehicle service record from the repository.
    /// </summary>
    /// <param name="serviceRecord">The vehicle service record entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default);
}
