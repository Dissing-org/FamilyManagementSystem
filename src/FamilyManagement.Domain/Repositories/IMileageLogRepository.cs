using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="MileageLogEntry"/> entities.
/// </summary>
public interface IMileageLogRepository
{
    /// <summary>
    /// Retrieves all mileage log entries for a specific vehicle.
    /// </summary>
    /// <param name="vehicleId">The unique identifier of the vehicle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of mileage log entries for the vehicle.</returns>
    Task<List<MileageLogEntry>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a mileage log entry by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the mileage log entry.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching mileage log entry or <c>null</c> if not found.</returns>
    Task<MileageLogEntry?> GetByIdAsync(MileageLogId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a new mileage log entry to the repository.
    /// </summary>
    /// <param name="logEntry">The mileage log entry entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a mileage log entry from the repository.
    /// </summary>
    /// <param name="logEntry">The mileage log entry entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default);
}
