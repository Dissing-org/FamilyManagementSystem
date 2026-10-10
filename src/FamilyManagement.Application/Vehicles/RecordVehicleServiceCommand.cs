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

public class RecordVehicleServiceCommandValidator
{
    public (bool IsValid, List<string> Errors) Validate(RecordVehicleServiceCommand command)
    {
        var errors = new List<string>();
        if (command.VehicleId == Guid.Empty)
            errors.Add("Vehicle ID is required.");
        if (string.IsNullOrWhiteSpace(command.Title))
            errors.Add("Service title is required.");
        if (command.MileageKm < 0)
            errors.Add("Mileage cannot be negative.");
        if (command.ServiceDate.Date > DateTime.UtcNow.Date.AddDays(1))
            errors.Add("Service date cannot be in the future.");
        if (command.Cost.HasValue && command.Cost.Value < 0)
            errors.Add("Cost cannot be negative.");
        return (errors.Count == 0, errors);
    }
}

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
