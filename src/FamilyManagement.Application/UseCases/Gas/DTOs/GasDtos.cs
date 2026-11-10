namespace FamilyManagement.Application.UseCases.Gas.DTOs;

public record GasReadingDto(
    Guid Id,
    DateTime RecordedDate,
    decimal ReadingM3,
    decimal? DeltaM3,
    decimal? EstimatedCostDkk,
    string? Notes,
    DateTime CreatedAt);

public record GasMonthlySeriesDto(
    int Year,
    int Month,
    decimal ConsumedM3,
    decimal TotalCostDkk,
    decimal EffectivePricePerM3,
    bool IsProjected);

public record GasDashboardSummaryDto(
    decimal CurrentMonthUsageM3,
    decimal CurrentMonthProjectedCostDkk,
    decimal DailyAverageM3,
    decimal EffectiveRatePerM3,
    decimal CommodityPricePerM3,
    decimal DistributionTaxPerM3,
    decimal MonthlySubscription,
    DateTime? LastPriceSyncedAt,
    List<GasMonthlySeriesDto> MonthlySeries,
    List<GasReadingDto> RecentReadings);

public record GasPricingSettingsDto(
    decimal CommodityPricePerM3,
    decimal DistributionTaxPerM3,
    decimal MonthlySubscription,
    decimal TotalPricePerM3,
    DateTime? LastSyncedAt,
    DateTime UpdatedAt);
