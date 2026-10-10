using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record UpdateVehicleDetailsCommand(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate,
    FuelType FuelType,
    string? Vin = null,
    int? ServiceIntervalKm = null,
    int? ServiceIntervalMonths = null,
    DateTime? NextInspectionDate = null);

public class UpdateVehicleDetailsCommandValidator
{
    public (bool IsValid, List<string> Errors) Validate(UpdateVehicleDetailsCommand command)
    {
        var errors = new List<string>();
        if (command.Id == Guid.Empty)
            errors.Add("Vehicle ID is required.");
        if (string.IsNullOrWhiteSpace(command.Make))
            errors.Add("Make is required.");
        if (string.IsNullOrWhiteSpace(command.Model))
            errors.Add("Model is required.");
        if (string.IsNullOrWhiteSpace(command.LicensePlate))
            errors.Add("License plate is required.");
        if (command.Year <= 1900)
            errors.Add("Year must be greater than 1900.");
        if (command.ServiceIntervalKm.HasValue && command.ServiceIntervalKm.Value <= 0)
            errors.Add("Service interval (km) must be greater than zero.");
        if (command.ServiceIntervalMonths.HasValue && command.ServiceIntervalMonths.Value <= 0)
            errors.Add("Service interval (months) must be greater than zero.");
        return (errors.Count == 0, errors);
    }
}

public class UpdateVehicleDetailsCommandHandler
{
    private readonly IVehicleRepository _repository;

    public UpdateVehicleDetailsCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<VehicleDto> HandleAsync(UpdateVehicleDetailsCommand command, CancellationToken cancellationToken = default)
    {
        var vehicle = await _repository.GetByIdAsync(VehicleId.From(command.Id), cancellationToken);
        if (vehicle is null)
        {
            throw new KeyNotFoundException($"Vehicle with ID '{command.Id}' was not found.");
        }

        var nextInspectionDateUtc = command.NextInspectionDate.HasValue
            ? DateTime.SpecifyKind(command.NextInspectionDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        vehicle.UpdateDetails(
            command.Make,
            command.Model,
            command.Year,
            command.LicensePlate,
            command.FuelType,
            command.Vin);

        vehicle.UpdateServiceSchedule(
            command.ServiceIntervalKm,
            command.ServiceIntervalMonths,
            nextInspectionDateUtc);

        await _repository.UpdateAsync(vehicle, cancellationToken);

        return VehicleDto.FromDomain(vehicle);
    }
}
