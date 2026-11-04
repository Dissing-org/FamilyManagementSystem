using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to create a new vehicle.
/// </summary>
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

/// <summary>
/// Handler for creating a new vehicle.
/// </summary>
public class CreateVehicleCommandHandler
{
    private readonly IVehicleRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateVehicleCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle repository.</param>
    public CreateVehicleCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the creation of a new vehicle.
    /// </summary>
    /// <param name="command">The command containing the vehicle details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the created vehicle.</returns>
    /// <exception cref="ArgumentException">Thrown when vehicle data is invalid.</exception>
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
