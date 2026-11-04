using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to update the details and service intervals of an existing vehicle.
/// </summary>
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

/// <summary>
/// Handler for updating vehicle details.
/// </summary>
public class UpdateVehicleDetailsCommandHandler
{
    private readonly IVehicleRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateVehicleDetailsCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle repository.</param>
    public UpdateVehicleDetailsCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles updating the details and service schedule of an existing vehicle.
    /// </summary>
    /// <param name="command">The command containing updated vehicle details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the updated vehicle.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the vehicle with the specified identifier is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when vehicle details or service intervals are invalid.</exception>
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
