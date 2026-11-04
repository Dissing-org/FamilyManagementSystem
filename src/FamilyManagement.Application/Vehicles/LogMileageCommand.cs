using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to log a new mileage entry for a vehicle.
/// </summary>
public record LogMileageCommand(
    Guid VehicleId,
    DateTime RecordedDate,
    int MileageKm,
    string? Notes = null);

/// <summary>
/// Handler for logging a new mileage entry for a vehicle.
/// </summary>
public class LogMileageCommandHandler
{
    private readonly IMileageLogRepository _mileageLogRepository;
    private readonly IVehicleRepository _vehicleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogMileageCommandHandler"/> class.
    /// </summary>
    /// <param name="mileageLogRepository">The mileage log repository.</param>
    /// <param name="vehicleRepository">The vehicle repository.</param>
    public LogMileageCommandHandler(
        IMileageLogRepository mileageLogRepository,
        IVehicleRepository vehicleRepository)
    {
        _mileageLogRepository = mileageLogRepository;
        _vehicleRepository = vehicleRepository;
    }

    /// <summary>
    /// Handles logging a new mileage entry for a vehicle and updates the vehicle's current mileage if greater.
    /// </summary>
    /// <param name="command">The command containing mileage log details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the created mileage log entry.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the vehicle with the specified identifier is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when mileage is negative or the recorded date is in the future.</exception>
    public async Task<MileageLogDto> HandleAsync(LogMileageCommand command, CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(command.VehicleId);
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
        {
            throw new KeyNotFoundException($"Vehicle with ID '{command.VehicleId}' was not found.");
        }

        var recordedDateUtc = DateTime.SpecifyKind(command.RecordedDate, DateTimeKind.Utc);

        var entry = MileageLogEntry.Create(
            vehicle.Id,
            recordedDateUtc,
            command.MileageKm,
            command.Notes);

        await _mileageLogRepository.AddAsync(entry, cancellationToken);

        // Update vehicle's CurrentMileageKm if the logged mileage is greater
        if (command.MileageKm > vehicle.CurrentMileageKm)
        {
            vehicle.UpdateCurrentMileage(command.MileageKm);
            await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        }

        return MileageLogDto.FromDomain(entry);
    }
}
