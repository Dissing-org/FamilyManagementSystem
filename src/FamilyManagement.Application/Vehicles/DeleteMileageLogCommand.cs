using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to delete a mileage log entry for a vehicle.
/// </summary>
public record DeleteMileageLogCommand(Guid VehicleId, Guid LogId);

/// <summary>
/// Handler for deleting a mileage log entry.
/// </summary>
public class DeleteMileageLogCommandHandler
{
    private readonly IMileageLogRepository _mileageLogRepository;
    private readonly IVehicleRepository _vehicleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteMileageLogCommandHandler"/> class.
    /// </summary>
    /// <param name="mileageLogRepository">The mileage log repository.</param>
    /// <param name="vehicleRepository">The vehicle repository.</param>
    public DeleteMileageLogCommandHandler(
        IMileageLogRepository mileageLogRepository,
        IVehicleRepository vehicleRepository)
    {
        _mileageLogRepository = mileageLogRepository;
        _vehicleRepository = vehicleRepository;
    }

    /// <summary>
    /// Handles the deletion of a mileage log entry and recalculates the vehicle's current mileage if necessary.
    /// </summary>
    /// <param name="command">The command containing the vehicle and mileage log identifiers.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the mileage log entry was found and deleted; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> HandleAsync(DeleteMileageLogCommand command, CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(command.VehicleId);
        var log = await _mileageLogRepository.GetByIdAsync(MileageLogId.From(command.LogId), cancellationToken);
        if (log is null || log.VehicleId != vehicleId)
        {
            return false;
        }

        await _mileageLogRepository.DeleteAsync(log, cancellationToken);

        // Recalculate and synchronize vehicle's current mileage from remaining history
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle != null)
        {
            var remainingLogs = await _mileageLogRepository.GetByVehicleIdAsync(vehicleId, cancellationToken);
            var maxRemainingMileage = remainingLogs.Count > 0 ? remainingLogs.Max(l => l.MileageKm) : 0;

            if (maxRemainingMileage < vehicle.CurrentMileageKm)
            {
                vehicle.UpdateCurrentMileage(maxRemainingMileage, allowDecrease: true);
                await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
            }
        }

        return true;
    }
}
