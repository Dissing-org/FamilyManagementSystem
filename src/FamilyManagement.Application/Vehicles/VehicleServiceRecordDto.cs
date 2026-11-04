using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Data transfer object representing a vehicle service or maintenance record.
/// </summary>
public record VehicleServiceRecordDto(
    Guid Id,
    Guid VehicleId,
    DateTime ServiceDate,
    int MileageKm,
    string Type,
    string Title,
    string? Workshop,
    decimal? Cost,
    string? Notes,
    Guid? ReceiptId,
    DateTime CreatedAt)
{
    /// <summary>
    /// Creates a <see cref="VehicleServiceRecordDto"/> from a <see cref="VehicleServiceRecord"/> domain entity.
    /// </summary>
    /// <param name="record">The vehicle service domain entity to map from.</param>
    /// <returns>A populated <see cref="VehicleServiceRecordDto"/> instance.</returns>
    public static VehicleServiceRecordDto FromDomain(VehicleServiceRecord record) =>
        new(
            record.Id.Value,
            record.VehicleId.Value,
            record.ServiceDate,
            record.MileageKm,
            record.Type.ToString(),
            record.Title,
            record.Workshop,
            record.Cost,
            record.Notes,
            record.ReceiptId,
            record.CreatedAt);
}
