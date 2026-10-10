using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.Vehicles;

public record CreateVehicleCommand(
    string Make,
    string Model,
    int Year,
    string LicensePlate,
    FuelType FuelType,
    int CurrentMileageKm = 0,
    string? Vin = null,
    int? ServiceIntervalKm = null,
    int? ServiceIntervalMonths = null,
    DateTime? NextInspectionDate = null);

public class CreateVehicleCommandValidator
{
    public (bool IsValid, List<string> Errors) Validate(CreateVehicleCommand command)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(command.Make))
            errors.Add("Make is required.");
        if (string.IsNullOrWhiteSpace(command.Model))
            errors.Add("Model is required.");
        if (string.IsNullOrWhiteSpace(command.LicensePlate))
            errors.Add("License plate is required.");
        if (command.Year <= 1900)
            errors.Add("Year must be greater than 1900.");
        if (command.CurrentMileageKm < 0)
            errors.Add("Current mileage cannot be negative.");
        if (command.ServiceIntervalKm.HasValue && command.ServiceIntervalKm.Value <= 0)
            errors.Add("Service interval (km) must be greater than zero.");
        if (command.ServiceIntervalMonths.HasValue && command.ServiceIntervalMonths.Value <= 0)
            errors.Add("Service interval (months) must be greater than zero.");
        return (errors.Count == 0, errors);
    }
}

public class CreateVehicleCommandHandler
{
    private readonly IVehicleRepository _repository;

    public CreateVehicleCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<VehicleDto> HandleAsync(CreateVehicleCommand command, CancellationToken cancellationToken = default)
    {
        var nextInspectionDateUtc = command.NextInspectionDate.HasValue
            ? DateTime.SpecifyKind(command.NextInspectionDate.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        var vehicle = Vehicle.Create(
            command.Make,
            command.Model,
            command.Year,
            command.LicensePlate,
            command.FuelType,
            command.CurrentMileageKm,
            command.Vin,
            command.ServiceIntervalKm,
            command.ServiceIntervalMonths,
            nextInspectionDateUtc);

        await _repository.AddAsync(vehicle, cancellationToken);

        return VehicleDto.FromDomain(vehicle);
    }
}
