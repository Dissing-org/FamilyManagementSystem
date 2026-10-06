using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class GrowthMeasurement
{
    public GrowthMeasurementId Id { get; private set; }
    public ChildId ChildId { get; private set; }
    public DateTime RecordedDate { get; private set; }
    public decimal? HeightCm { get; private set; }
    public decimal? WeightKg { get; private set; }
    public decimal? HeadCircumferenceCm { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core parameterless constructor
    private GrowthMeasurement()
    {
        Id = null!;
        ChildId = null!;
    }

    private GrowthMeasurement(
        GrowthMeasurementId id,
        ChildId childId,
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm,
        string? notes,
        DateTime createdAt)
    {
        Id = id;
        ChildId = childId;
        RecordedDate = DateTime.SpecifyKind(recordedDate, DateTimeKind.Utc);
        HeightCm = heightCm;
        WeightKg = weightKg;
        HeadCircumferenceCm = headCircumferenceCm;
        Notes = notes?.Trim();
        CreatedAt = createdAt;
    }

    public static GrowthMeasurement Create(
        ChildId childId,
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm = null,
        string? notes = null)
    {
        if (childId is null)
        {
            throw new ArgumentNullException(nameof(childId));
        }

        if (heightCm is null && weightKg is null && headCircumferenceCm is null)
        {
            throw new ArgumentException("At least one measurement (height, weight, or head circumference) must be provided.");
        }

        if (heightCm.HasValue && heightCm.Value <= 0)
        {
            throw new ArgumentException("Height must be greater than zero.", nameof(heightCm));
        }

        if (weightKg.HasValue && weightKg.Value <= 0)
        {
            throw new ArgumentException("Weight must be greater than zero.", nameof(weightKg));
        }

        if (headCircumferenceCm.HasValue && headCircumferenceCm.Value <= 0)
        {
            throw new ArgumentException("Head circumference must be greater than zero.", nameof(headCircumferenceCm));
        }

        return new GrowthMeasurement(
            GrowthMeasurementId.New(),
            childId,
            recordedDate,
            heightCm,
            weightKg,
            headCircumferenceCm,
            notes,
            DateTime.UtcNow);
    }

    public void Update(
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm,
        string? notes)
    {
        if (heightCm is null && weightKg is null && headCircumferenceCm is null)
        {
            throw new ArgumentException("At least one measurement must be provided.");
        }

        if (heightCm.HasValue && heightCm.Value <= 0)
        {
            throw new ArgumentException("Height must be greater than zero.", nameof(heightCm));
        }

        if (weightKg.HasValue && weightKg.Value <= 0)
        {
            throw new ArgumentException("Weight must be greater than zero.", nameof(weightKg));
        }

        if (headCircumferenceCm.HasValue && headCircumferenceCm.Value <= 0)
        {
            throw new ArgumentException("Head circumference must be greater than zero.", nameof(headCircumferenceCm));
        }

        RecordedDate = DateTime.SpecifyKind(recordedDate, DateTimeKind.Utc);
        HeightCm = heightCm;
        WeightKg = weightKg;
        HeadCircumferenceCm = headCircumferenceCm;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
