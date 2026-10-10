using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class MileageLogEntry
{
    public MileageLogId Id { get; private set; }
    public VehicleId VehicleId { get; private set; }
    public DateTime RecordedDate { get; private set; }
    public int MileageKm { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // EF Core parameterless constructor
    private MileageLogEntry()
    {
        Id = null!;
        VehicleId = null!;
    }

    private MileageLogEntry(
        MileageLogId id,
        VehicleId vehicleId,
        DateTime recordedDate,
        int mileageKm,
        string? notes,
        DateTime createdAt)
    {
        Id = id;
        VehicleId = vehicleId;
        RecordedDate = DateTime.SpecifyKind(recordedDate, DateTimeKind.Utc);
        MileageKm = mileageKm;
        Notes = notes?.Trim();
        CreatedAt = createdAt;
    }

    public static MileageLogEntry Create(
        VehicleId vehicleId,
        DateTime recordedDate,
        int mileageKm,
        string? notes = null)
    {
        if (vehicleId is null)
        {
            throw new ArgumentNullException(nameof(vehicleId));
        }

        if (mileageKm < 0)
        {
            throw new ArgumentException("Mileage cannot be negative.", nameof(mileageKm));
        }

        if (recordedDate.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Recorded date cannot be in the future.", nameof(recordedDate));
        }

        return new MileageLogEntry(
            MileageLogId.New(),
            vehicleId,
            recordedDate,
            mileageKm,
            notes,
            DateTime.UtcNow);
    }
}
