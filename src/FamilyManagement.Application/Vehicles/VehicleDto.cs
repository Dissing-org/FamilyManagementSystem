using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Data transfer object representing a vehicle.
/// </summary>
public record VehicleDto(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate,
    string? Vin,
    string FuelType,
    int CurrentMileageKm,
    int? ServiceIntervalKm,
    int? ServiceIntervalMonths,
    DateTime? NextInspectionDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    /// <summary>
    /// Creates a <see cref="VehicleDto"/> from a <see cref="Vehicle"/> domain entity.
    /// </summary>
    /// <param name="vehicle">The vehicle domain entity to map from.</param>
    /// <returns>A populated <see cref="VehicleDto"/> instance.</returns>
    public static VehicleDto FromDomain(Vehicle vehicle) =>
        new(
            vehicle.Id.Value,
            vehicle.Make,
            vehicle.Model,
            vehicle.Year,
            vehicle.LicensePlate,
            vehicle.Vin,
            vehicle.FuelType.ToString(),
            vehicle.CurrentMileageKm,
            vehicle.ServiceIntervalKm,
            vehicle.ServiceIntervalMonths,
            vehicle.NextInspectionDate,
            vehicle.CreatedAt,
            vehicle.UpdatedAt);
}
