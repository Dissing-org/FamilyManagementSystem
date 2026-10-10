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
