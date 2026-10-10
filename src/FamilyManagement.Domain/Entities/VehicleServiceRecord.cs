using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class VehicleServiceRecord
{
    public ServiceRecordId Id { get; private set; }
    public VehicleId VehicleId { get; private set; }
    public DateTime ServiceDate { get; private set; }
    public int MileageKm { get; private set; }
    public ServiceType Type { get; private set; }
    public string Title { get; private set; }
    public string? Workshop { get; private set; }
    public decimal? Cost { get; private set; }
    public string? Notes { get; private set; }
    public Guid? ReceiptId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // EF Core parameterless constructor
    private VehicleServiceRecord()
    {
        Id = null!;
        VehicleId = null!;
        Title = null!;
    }

    private VehicleServiceRecord(
        ServiceRecordId id,
        VehicleId vehicleId,
        DateTime serviceDate,
        int mileageKm,
        ServiceType type,
        string title,
        string? workshop,
        decimal? cost,
        string? notes,
        Guid? receiptId,
        DateTime createdAt)
    {
        Id = id;
        VehicleId = vehicleId;
        ServiceDate = DateTime.SpecifyKind(serviceDate, DateTimeKind.Utc);
        MileageKm = mileageKm;
        Type = type;
        Title = title;
        Workshop = workshop;
        Cost = cost;
        Notes = notes;
        ReceiptId = receiptId;
        CreatedAt = createdAt;
    }

    public static VehicleServiceRecord Create(
        VehicleId vehicleId,
        DateTime serviceDate,
        int mileageKm,
        ServiceType type,
        string title,
        string? workshop = null,
        decimal? cost = null,
        string? notes = null,
        Guid? receiptId = null)
    {
        if (vehicleId is null)
        {
            throw new ArgumentNullException(nameof(vehicleId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Service title is required.", nameof(title));
        }

        if (mileageKm < 0)
        {
            throw new ArgumentException("Mileage cannot be negative.", nameof(mileageKm));
        }

        if (serviceDate.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Service date cannot be in the future.", nameof(serviceDate));
        }

        if (cost.HasValue && cost.Value < 0)
        {
            throw new ArgumentException("Cost cannot be negative.", nameof(cost));
        }

        return new VehicleServiceRecord(
            ServiceRecordId.New(),
            vehicleId,
            serviceDate,
            mileageKm,
            type,
            title.Trim(),
            workshop?.Trim(),
            cost,
            notes?.Trim(),
            receiptId,
            DateTime.UtcNow);
    }
}
