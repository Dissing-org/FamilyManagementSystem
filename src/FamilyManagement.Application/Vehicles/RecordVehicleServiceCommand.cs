using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to record a service or maintenance event for a vehicle.
/// </summary>
public record RecordVehicleServiceCommand(
    Guid VehicleId,
    DateTime ServiceDate,
    int MileageKm,
    ServiceType Type,
    string Title,
    string? Workshop = null,
    decimal? Cost = null,
    string? Notes = null,
    Guid? ReceiptId = null);

/// <summary>
/// Handler for recording a vehicle service event.
/// </summary>
public class RecordVehicleServiceCommandHandler
{
    private readonly IVehicleServiceRepository _serviceRepository;
    private readonly IVehicleRepository _vehicleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordVehicleServiceCommandHandler"/> class.
    /// </summary>
    /// <param name="serviceRepository">The vehicle service repository.</param>
    /// <param name="vehicleRepository">The vehicle repository.</param>
    public RecordVehicleServiceCommandHandler(
        IVehicleServiceRepository serviceRepository,
        IVehicleRepository vehicleRepository)
    {
        _serviceRepository = serviceRepository;
        _vehicleRepository = vehicleRepository;
    }

    /// <summary>
    /// Handles recording a new service record for a vehicle and updates the vehicle's current mileage if greater.
    /// </summary>
    /// <param name="command">The command containing service record details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the recorded vehicle service.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the vehicle with the specified identifier is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when service details, mileage, date, or cost are invalid.</exception>
    public async Task<VehicleServiceRecordDto> HandleAsync(
        RecordVehicleServiceCommand command,
        CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(command.VehicleId);
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
        {
            throw new KeyNotFoundException($"Vehicle with ID '{command.VehicleId}' was not found.");
        }

        var serviceDateUtc = DateTime.SpecifyKind(command.ServiceDate, DateTimeKind.Utc);

        var record = VehicleServiceRecord.Create(
            vehicle.Id,
            serviceDateUtc,
            command.MileageKm,
            command.Type,
            command.Title,
            command.Workshop,
            command.Cost,
            command.Notes,
            command.ReceiptId);

        await _serviceRepository.AddAsync(record, cancellationToken);

        // Update vehicle's CurrentMileageKm if service mileage is greater
        if (command.MileageKm > vehicle.CurrentMileageKm)
        {
            vehicle.UpdateCurrentMileage(command.MileageKm);
            await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        }

        return VehicleServiceRecordDto.FromDomain(record);
    }
}
