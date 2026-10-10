using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

public record MileageLogDto(
    Guid Id,
    Guid VehicleId,
    DateTime RecordedDate,
    int MileageKm,
    string? Notes,
    DateTime CreatedAt)
{
    public static MileageLogDto FromDomain(MileageLogEntry entry) =>
        new(
            entry.Id.Value,
            entry.VehicleId.Value,
            entry.RecordedDate,
            entry.MileageKm,
            entry.Notes,
            entry.CreatedAt);
}
