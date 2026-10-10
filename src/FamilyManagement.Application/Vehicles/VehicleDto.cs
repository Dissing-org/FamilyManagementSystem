using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

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
