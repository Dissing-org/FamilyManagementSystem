using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

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

public class RecordVehicleServiceCommandHandler
{
    private readonly IVehicleServiceRepository _serviceRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public RecordVehicleServiceCommandHandler(
        IVehicleServiceRepository serviceRepository,
        IVehicleRepository vehicleRepository)
    {
        _serviceRepository = serviceRepository;
        _vehicleRepository = vehicleRepository;
    }

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
