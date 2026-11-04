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

    /// <summary>
    /// Creates a new <see cref="MileageLogEntry"/> for tracking a vehicle's odometer reading over time.
    /// </summary>
    /// <param name="vehicleId">The identifier of the vehicle.</param>
    /// <param name="recordedDate">The date and time when the mileage was recorded.</param>
    /// <param name="mileageKm">The odometer reading in kilometers.</param>
    /// <param name="notes">Optional notes or remarks regarding the entry.</param>
    /// <returns>A new <see cref="MileageLogEntry"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="vehicleId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="mileageKm"/> is negative, or when <paramref name="recordedDate"/> is in the future.</exception>
    public static MileageLogEntry Create(
        VehicleId vehicleId,
        DateTime recordedDate,
        int mileageKm,
        string? notes = null)
    {
        ArgumentNullException.ThrowIfNull(vehicleId);

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
