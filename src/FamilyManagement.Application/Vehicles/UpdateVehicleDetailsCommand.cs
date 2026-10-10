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
