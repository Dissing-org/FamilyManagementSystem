using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Data transfer object representing a vehicle mileage log entry.
/// </summary>
public record MileageLogDto(
    Guid Id,
    Guid VehicleId,
    DateTime RecordedDate,
    int MileageKm,
    string? Notes,
    DateTime CreatedAt)
{
    /// <summary>
    /// Creates a <see cref="MileageLogDto"/> from a <see cref="MileageLogEntry"/> domain entity.
    /// </summary>
    /// <param name="entry">The mileage log domain entity to map from.</param>
    /// <returns>A populated <see cref="MileageLogDto"/> instance.</returns>
    public static MileageLogDto FromDomain(MileageLogEntry entry) =>
        new(
            entry.Id.Value,
            entry.VehicleId.Value,
            entry.RecordedDate,
            entry.MileageKm,
            entry.Notes,
            entry.CreatedAt);
}
