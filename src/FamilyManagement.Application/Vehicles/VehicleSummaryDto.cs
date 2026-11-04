using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Data transfer object representing a comprehensive summary of a vehicle, including maintenance schedules and alerts.
/// </summary>
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
    decimal TotalMaintenanceCost)
{
    /// <summary>
    /// Creates a <see cref="VehicleSummaryDto"/> from vehicle data, mileage logs, and service records.
    /// </summary>
    /// <param name="vehicle">The vehicle domain entity.</param>
    /// <param name="mileageLogs">The list of mileage log entries for the vehicle.</param>
    /// <param name="serviceRecords">The list of service records for the vehicle.</param>
    /// <param name="asOfDate">The reference date to evaluate due dates against; defaults to UTC now if omitted.</param>
    /// <returns>A populated <see cref="VehicleSummaryDto"/> instance with evaluated service schedules and alerts.</returns>
    public static VehicleSummaryDto Create(
        Vehicle vehicle,
        List<MileageLogEntry> mileageLogs,
        List<VehicleServiceRecord> serviceRecords,
        DateTime? asOfDate = null)
    {
        var today = (asOfDate ?? DateTime.UtcNow).Date;
        var latestService = serviceRecords
            .Where(s => s.Type is FamilyManagement.Domain.Enums.ServiceType.RegularService or FamilyManagement.Domain.Enums.ServiceType.OilChange)
            .OrderByDescending(s => s.ServiceDate)
            .FirstOrDefault();

        DateTime? lastServiceDate = latestService?.ServiceDate;
        int? lastServiceMileage = latestService?.MileageKm;

        int? kmUntilNextService = null;
        if (vehicle.ServiceIntervalKm.HasValue)
        {
            var baseMileage = lastServiceMileage ?? vehicle.CurrentMileageKm;
            var targetMileage = baseMileage + vehicle.ServiceIntervalKm.Value;
            kmUntilNextService = targetMileage - vehicle.CurrentMileageKm;
        }

        int? daysUntilNextService = null;
        DateTime? nextServiceDueDate = null;
        if (vehicle.ServiceIntervalMonths.HasValue)
        {
            var baseDate = lastServiceDate ?? vehicle.CreatedAt;
            nextServiceDueDate = baseDate.AddMonths(vehicle.ServiceIntervalMonths.Value);
            daysUntilNextService = (int)(nextServiceDueDate.Value.Date - today).TotalDays;
        }

        int? daysUntilNextInspection = null;
        if (vehicle.NextInspectionDate.HasValue)
        {
            daysUntilNextInspection = (int)(vehicle.NextInspectionDate.Value.Date - today).TotalDays;
        }

        var alerts = new List<string>();
        bool inspectionDue = false;
        if (daysUntilNextInspection.HasValue)
        {
            if (daysUntilNextInspection < 0)
            {
                alerts.Add("InspectionOverdue");
                inspectionDue = true;
            }
            else if (daysUntilNextInspection <= 30)
            {
                alerts.Add("InspectionDue");
                inspectionDue = true;
            }
        }

        bool serviceDue = false;
        if ((kmUntilNextService.HasValue && kmUntilNextService < 0) ||
            (daysUntilNextService.HasValue && daysUntilNextService < 0))
        {
            serviceDue = true;
            alerts.Add("ServiceOverdue");
        }
        else if ((kmUntilNextService.HasValue && kmUntilNextService <= 1000) ||
                 (daysUntilNextService.HasValue && daysUntilNextService <= 30))
        {
            serviceDue = true;
            alerts.Add("ServiceDue");
        }

        decimal totalCost = serviceRecords
            .Where(s => s.Cost.HasValue)
            .Sum(s => s.Cost!.Value);

        return new VehicleSummaryDto(
            VehicleDto.FromDomain(vehicle),
            vehicle.CurrentMileageKm,
            lastServiceDate,
            lastServiceMileage,
            kmUntilNextService,
            daysUntilNextService,
            nextServiceDueDate,
            vehicle.NextInspectionDate,
            daysUntilNextInspection,
            serviceDue,
            inspectionDue,
            alerts,
            serviceRecords.Count,
            mileageLogs.Count,
            totalCost);
    }
}
