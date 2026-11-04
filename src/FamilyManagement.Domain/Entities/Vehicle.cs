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

    /// <summary>
    /// Creates a new <see cref="Vehicle"/> instance with the specified specification and maintenance tracking details.
    /// </summary>
    /// <param name="make">The manufacturer or brand of the vehicle.</param>
    /// <param name="model">The model designation of the vehicle.</param>
    /// <param name="year">The manufacturing year of the vehicle.</param>
    /// <param name="licensePlate">The registration license plate number.</param>
    /// <param name="fuelType">The propulsion/fuel type used by the vehicle.</param>
    /// <param name="currentMileageKm">The current odometer mileage in kilometers. Defaults to 0.</param>
    /// <param name="vin">The optional Vehicle Identification Number (VIN).</param>
    /// <param name="serviceIntervalKm">The optional recommended service interval in kilometers.</param>
    /// <param name="serviceIntervalMonths">The optional recommended service interval in months.</param>
    /// <param name="nextInspectionDate">The optional next scheduled inspection date.</param>
    /// <returns>A new <see cref="Vehicle"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="make"/>, <paramref name="model"/>, or <paramref name="licensePlate"/> is null or whitespace, when <paramref name="year"/> is 1900 or earlier, when <paramref name="currentMileageKm"/> is negative, or when <paramref name="serviceIntervalKm"/> or <paramref name="serviceIntervalMonths"/> is less than or equal to zero.</exception>
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

    /// <summary>
    /// Updates the core vehicle specifications including make, model, year, license plate, fuel type, and VIN.
    /// </summary>
    /// <param name="make">The updated vehicle make.</param>
    /// <param name="model">The updated vehicle model.</param>
    /// <param name="year">The updated manufacturing year.</param>
    /// <param name="licensePlate">The updated license plate number.</param>
    /// <param name="fuelType">The updated fuel type.</param>
    /// <param name="vin">The updated optional Vehicle Identification Number.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="make"/>, <paramref name="model"/>, or <paramref name="licensePlate"/> is null or whitespace, or when <paramref name="year"/> is 1900 or earlier.</exception>
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

    /// <summary>
    /// Updates the current odometer mileage for the vehicle.
    /// </summary>
    /// <param name="newMileageKm">The new mileage reading in kilometers.</param>
    /// <param name="allowDecrease">Whether to permit a decrease in mileage (e.g., in case of data entry correction or odometer change). Defaults to false.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newMileageKm"/> is negative, or when <paramref name="allowDecrease"/> is false and <paramref name="newMileageKm"/> is less than the current mileage.</exception>
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

    /// <summary>
    /// Updates the service schedule intervals and the next inspection date.
    /// </summary>
    /// <param name="intervalKm">The optional service interval in kilometers.</param>
    /// <param name="intervalMonths">The optional service interval in months.</param>
    /// <param name="nextInspectionDate">The optional next inspection date.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="intervalKm"/> or <paramref name="intervalMonths"/> is less than or equal to zero.</exception>
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
