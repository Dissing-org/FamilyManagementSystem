namespace FamilyManagement.UI.Shared.Models;

public record VehicleDto(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate,
    string? Vin,
    string FuelType,
    int CurrentMileageKm,
    int? ServiceIntervalKm,
    int? ServiceIntervalMonths,
    DateTime? NextInspectionDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record VehicleSummaryDto(
    VehicleDto Vehicle,
    int LatestMileageKm,
    DateTime? LastServiceDate,
    int? LastServiceMileageKm,
    int? KmUntilNextService,
    int? DaysUntilNextService,
    DateTime? NextServiceDueDate,
    DateTime? NextInspectionDate,
    int? DaysUntilNextInspection,
    bool ServiceDue,
    bool InspectionDue,
    List<string> StatusAlerts,
    int TotalServiceRecordsCount,
    int TotalMileageLogsCount,
    decimal TotalMaintenanceCost);

public record MileageLogDto(
    Guid Id,
    Guid VehicleId,
    DateTime RecordedDate,
    int MileageKm,
    string? Notes,
    DateTime CreatedAt);

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
    DateTime CreatedAt);

public class CreateVehicleRequest
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public string LicensePlate { get; set; } = string.Empty;
    public string FuelType { get; set; } = "Gasoline";
    public int CurrentMileageKm { get; set; } = 0;
    public string? Vin { get; set; }
    public int? ServiceIntervalKm { get; set; } = 15000;
    public int? ServiceIntervalMonths { get; set; } = 12;
    public DateTime? NextInspectionDate { get; set; }
}

public class UpdateVehicleRequest
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string FuelType { get; set; } = "Gasoline";
    public string? Vin { get; set; }
    public int? ServiceIntervalKm { get; set; }
    public int? ServiceIntervalMonths { get; set; }
    public DateTime? NextInspectionDate { get; set; }
}

public class LogMileageRequest
{
    public Guid Id { get; set; }
    public DateTime RecordedDate { get; set; } = DateTime.Today;
    public int MileageKm { get; set; }
    public string? Notes { get; set; }
}

public class RecordVehicleServiceRequest
{
    public Guid Id { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.Today;
    public int MileageKm { get; set; }
    public string Type { get; set; } = "RegularService";
    public string Title { get; set; } = string.Empty;
    public string? Workshop { get; set; }
    public decimal? Cost { get; set; }
    public string? Notes { get; set; }
    public Guid? ReceiptId { get; set; }
}
