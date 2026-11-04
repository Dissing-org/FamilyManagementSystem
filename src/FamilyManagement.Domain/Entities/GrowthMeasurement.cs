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

    /// <summary>
    /// Creates a new <see cref="GrowthMeasurement"/> instance for a child.
    /// </summary>
    /// <param name="childId">The identifier of the child.</param>
    /// <param name="recordedDate">The date and time when the measurement was taken.</param>
    /// <param name="heightCm">The optional measured height in centimeters.</param>
    /// <param name="weightKg">The optional measured weight in kilograms.</param>
    /// <param name="headCircumferenceCm">The optional measured head circumference in centimeters.</param>
    /// <param name="notes">Optional notes or remarks regarding the measurement.</param>
    /// <returns>A new <see cref="GrowthMeasurement"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="childId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recordedDate"/> is in the future, when no measurement value is provided, or when any provided measurement value is less than or equal to zero.</exception>
    public static GrowthMeasurement Create(
        ChildId childId,
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm = null,
        string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(childId);

        if (recordedDate.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Recorded date cannot be in the future.", nameof(recordedDate));
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

    /// <summary>
    /// Updates the measurement values, date, and notes for this growth record.
    /// </summary>
    /// <param name="recordedDate">The updated date and time of the measurement.</param>
    /// <param name="heightCm">The updated measured height in centimeters.</param>
    /// <param name="weightKg">The updated measured weight in kilograms.</param>
    /// <param name="headCircumferenceCm">The updated measured head circumference in centimeters.</param>
    /// <param name="notes">Updated optional notes or remarks.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recordedDate"/> is in the future, when no measurement value is provided, or when any provided measurement value is less than or equal to zero.</exception>
    public void Update(
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm,
        string? notes)
    {
        if (recordedDate.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Recorded date cannot be in the future.", nameof(recordedDate));
        }

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
