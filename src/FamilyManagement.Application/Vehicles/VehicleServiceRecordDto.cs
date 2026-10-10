using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

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
