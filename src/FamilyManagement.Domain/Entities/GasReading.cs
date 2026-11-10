using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class GasReading
{
    public GasReadingId Id { get; private set; }
    public DateTime RecordedDate { get; private set; }
    public decimal ReadingM3 { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private GasReading()
    {
        Id = null!;
    }

    private GasReading(
        GasReadingId id,
        DateTime recordedDate,
        decimal readingM3,
        string? notes,
        DateTime createdAt)
    {
        Id = id;
        RecordedDate = recordedDate;
        ReadingM3 = readingM3;
        Notes = notes;
        CreatedAt = createdAt;
    }

    public static GasReading Create(
        DateTime recordedDate,
        decimal readingM3,
        string? notes = null)
    {
        if (readingM3 < 0)
        {
            throw new ArgumentException("Meter reading cannot be negative.", nameof(readingM3));
        }

        var utcDate = recordedDate.Kind switch
        {
            DateTimeKind.Utc => recordedDate,
            DateTimeKind.Local => recordedDate.ToUniversalTime(),
            _ => DateTime.SpecifyKind(recordedDate, DateTimeKind.Utc)
        };

        if (utcDate > DateTime.UtcNow.AddMinutes(15))
        {
            throw new ArgumentException("Recorded date cannot be in the future.", nameof(recordedDate));
        }

        return new GasReading(
            GasReadingId.New(),
            utcDate,
            readingM3,
            notes?.Trim(),
            DateTime.UtcNow);
    }

    public void Update(DateTime recordedDate, decimal readingM3, string? notes)
    {
        if (readingM3 < 0)
        {
            throw new ArgumentException("Meter reading cannot be negative.", nameof(readingM3));
        }

        var utcDate = recordedDate.Kind switch
        {
            DateTimeKind.Utc => recordedDate,
            DateTimeKind.Local => recordedDate.ToUniversalTime(),
            _ => DateTime.SpecifyKind(recordedDate, DateTimeKind.Utc)
        };

        if (utcDate > DateTime.UtcNow.AddMinutes(15))
        {
            throw new ArgumentException("Recorded date cannot be in the future.", nameof(recordedDate));
        }

        RecordedDate = utcDate;
        ReadingM3 = readingM3;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
