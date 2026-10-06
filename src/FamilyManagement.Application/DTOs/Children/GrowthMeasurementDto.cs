using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Children;

public record GrowthMeasurementDto(
    Guid Id,
    Guid ChildId,
    DateTime RecordedDate,
    decimal? HeightCm,
    decimal? WeightKg,
    decimal? HeadCircumferenceCm,
    string? Notes,
    DateTime CreatedAt)
{
    public static GrowthMeasurementDto FromDomain(GrowthMeasurement measurement) =>
        new(
            measurement.Id.Value,
            measurement.ChildId.Value,
            measurement.RecordedDate,
            measurement.HeightCm,
            measurement.WeightKg,
            measurement.HeadCircumferenceCm,
            measurement.Notes,
            measurement.CreatedAt);
}
