using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record LogMileageCommand(
    Guid VehicleId,
    DateTime RecordedDate,
    int MileageKm,
    string? Notes = null);

public class LogMileageCommandValidator
{
    public (bool IsValid, List<string> Errors) Validate(LogMileageCommand command)
    {
        var errors = new List<string>();
        if (command.VehicleId == Guid.Empty)
            errors.Add("Vehicle ID is required.");
        if (command.MileageKm < 0)
            errors.Add("Mileage cannot be negative.");
        if (command.RecordedDate.Date > DateTime.UtcNow.Date.AddDays(1))
            errors.Add("Recorded date cannot be in the future.");
        return (errors.Count == 0, errors);
    }
}

public class LogMileageCommandHandler
{
    private readonly IMileageLogRepository _mileageLogRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public LogMileageCommandHandler(
        IMileageLogRepository mileageLogRepository,
        IVehicleRepository vehicleRepository)
    {
        _mileageLogRepository = mileageLogRepository;
        _vehicleRepository = vehicleRepository;
    }

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
