using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class Vehicle
{
    public VehicleId Id { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public string LicensePlate { get; private set; }
    public string? Vin { get; private set; }
    public FuelType FuelType { get; private set; }
    public int CurrentMileageKm { get; private set; }
    public int? ServiceIntervalKm { get; private set; }
    public int? ServiceIntervalMonths { get; private set; }
    public DateTime? NextInspectionDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core parameterless constructor
    private Vehicle()
    {
        Id = null!;
        Make = null!;
        Model = null!;
        LicensePlate = null!;
    }

    private Vehicle(
        VehicleId id,
        string make,
        string model,
        int year,
        string licensePlate,
        FuelType fuelType,
        int currentMileageKm,
        string? vin,
        int? serviceIntervalKm,
        int? serviceIntervalMonths,
        DateTime? nextInspectionDate,
        DateTime createdAt)
    {
        Id = id;
        Make = make;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
        FuelType = fuelType;
        CurrentMileageKm = currentMileageKm;
        Vin = vin;
        ServiceIntervalKm = serviceIntervalKm;
        ServiceIntervalMonths = serviceIntervalMonths;
        NextInspectionDate = nextInspectionDate;
        CreatedAt = createdAt;
    }

    public static Vehicle Create(
        string make,
        string model,
        int year,
        string licensePlate,
        FuelType fuelType,
        int currentMileageKm = 0,
        string? vin = null,
        int? serviceIntervalKm = null,
        int? serviceIntervalMonths = null,
        DateTime? nextInspectionDate = null)
    {
        if (string.IsNullOrWhiteSpace(make))
        {
            throw new ArgumentException("Make is required.", nameof(make));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("Model is required.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate is required.", nameof(licensePlate));
        }

        if (year <= 1900)
        {
            throw new ArgumentException("Year must be greater than 1900.", nameof(year));
        }

        if (currentMileageKm < 0)
        {
            throw new ArgumentException("Current mileage cannot be negative.", nameof(currentMileageKm));
        }

        if (serviceIntervalKm.HasValue && serviceIntervalKm.Value <= 0)
        {
            throw new ArgumentException("Service interval (km) must be greater than zero.", nameof(serviceIntervalKm));
        }

        if (serviceIntervalMonths.HasValue && serviceIntervalMonths.Value <= 0)
        {
            throw new ArgumentException("Service interval (months) must be greater than zero.", nameof(serviceIntervalMonths));
        }

        return new Vehicle(
            VehicleId.New(),
            make.Trim(),
            model.Trim(),
            year,
            licensePlate.Trim(),
            fuelType,
            currentMileageKm,
            vin?.Trim(),
            serviceIntervalKm,
            serviceIntervalMonths,
            nextInspectionDate.HasValue ? DateTime.SpecifyKind(nextInspectionDate.Value, DateTimeKind.Utc) : null,
            DateTime.UtcNow);
    }

    public void UpdateDetails(
        string make,
        string model,
        int year,
        string licensePlate,
        FuelType fuelType,
        string? vin)
    {
        if (string.IsNullOrWhiteSpace(make))
        {
            throw new ArgumentException("Make is required.", nameof(make));
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("Model is required.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("License plate is required.", nameof(licensePlate));
        }

        if (year <= 1900)
        {
            throw new ArgumentException("Year must be greater than 1900.", nameof(year));
        }

        Make = make.Trim();
        Model = model.Trim();
        Year = year;
        LicensePlate = licensePlate.Trim();
        FuelType = fuelType;
        Vin = vin?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCurrentMileage(int newMileageKm, bool allowDecrease = false)
    {
        if (newMileageKm < 0)
        {
            throw new ArgumentException("Mileage cannot be negative.", nameof(newMileageKm));
        }

        if (!allowDecrease && newMileageKm < CurrentMileageKm)
        {
            throw new ArgumentException($"New mileage ({newMileageKm} km) cannot be less than current mileage ({CurrentMileageKm} km).", nameof(newMileageKm));
        }

        CurrentMileageKm = newMileageKm;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateServiceSchedule(int? intervalKm, int? intervalMonths, DateTime? nextInspectionDate)
    {
        if (intervalKm.HasValue && intervalKm.Value <= 0)
        {
            throw new ArgumentException("Service interval (km) must be greater than zero.", nameof(intervalKm));
        }

        if (intervalMonths.HasValue && intervalMonths.Value <= 0)
        {
            throw new ArgumentException("Service interval (months) must be greater than zero.", nameof(intervalMonths));
        }

        ServiceIntervalKm = intervalKm;
        ServiceIntervalMonths = intervalMonths;
        NextInspectionDate = nextInspectionDate.HasValue
            ? DateTime.SpecifyKind(nextInspectionDate.Value, DateTimeKind.Utc)
            : null;
        UpdatedAt = DateTime.UtcNow;
    }
}
