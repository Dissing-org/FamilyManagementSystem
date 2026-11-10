using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.Services;

namespace FamilyManagement.Application.UseCases.Gas.Queries;

public record GetGasDashboardQuery;

public class GetGasDashboardQueryHandler
{
    private readonly IGasReadingRepository _readingRepository;
    private readonly IGasPricingRepository _pricingRepository;

    public GetGasDashboardQueryHandler(
        IGasReadingRepository readingRepository,
        IGasPricingRepository pricingRepository)
    {
        _readingRepository = readingRepository;
        _pricingRepository = pricingRepository;
    }

    public async Task<GasDashboardSummaryDto> HandleAsync(GetGasDashboardQuery query, CancellationToken ct = default)
    {
        var readings = await _readingRepository.GetAllAsync(ct);
        var settings = await _pricingRepository.GetSettingsAsync(ct);
        var snapshots = await _pricingRepository.GetAllSnapshotsAsync(ct);
        var snapshotMap = snapshots.ToDictionary(s => (s.Year, s.Month), s => s);

        var now = DateTime.UtcNow;
        var currentMonthKey = (now.Year, now.Month);

        // Apportioned consumption per (Year, Month)
        var monthlyMap = GasConsumptionCalculator.ApportionMonthlyConsumption(readings);
        var currentMonthActualM3 = monthlyMap.GetValueOrDefault(currentMonthKey, 0m);

        // Calculate run-rate projection for current ongoing month
        var currentMonthProjection = GasConsumptionCalculator.CalculateProjectedCurrentMonth(
            currentMonthActualM3,
            now,
            settings);

        // Generate past 12 months series
        var monthlySeries = new List<GasMonthlySeriesDto>();
        for (int i = 11; i >= 0; i--)
        {
            var date = now.AddMonths(-i);
            var key = (date.Year, date.Month);
            var isCurrentMonth = (date.Year == now.Year && date.Month == now.Month);

            if (isCurrentMonth)
            {
                monthlySeries.Add(new GasMonthlySeriesDto(
                    date.Year,
                    date.Month,
                    Math.Round(currentMonthProjection.ConsumedM3, 1),
                    Math.Round(currentMonthProjection.TotalCostDkk, 2),
                    settings.TotalPricePerM3,
                    IsProjected: true));
            }
            else
            {
                var consumedM3 = Math.Round(monthlyMap.GetValueOrDefault(key, 0m), 1);
                var rate = snapshotMap.TryGetValue(key, out var snap) ? snap.TotalPricePerM3 : settings.TotalPricePerM3;
                var sub = snapshotMap.TryGetValue(key, out snap) ? snap.MonthlySubscription : settings.MonthlySubscription;
                var cost = consumedM3 > 0 ? Math.Round((consumedM3 * rate) + sub, 2) : 0m;

                monthlySeries.Add(new GasMonthlySeriesDto(
                    date.Year,
                    date.Month,
                    consumedM3,
                    cost,
                    rate,
                    IsProjected: false));
            }
        }

        // Daily average for current month
        var daysElapsed = Math.Max(1, now.Day);
        var dailyAverageM3 = Math.Round(currentMonthActualM3 / daysElapsed, 2);

        // Build recent readings
        var ordered = readings.OrderBy(r => r.RecordedDate).ToList();
        var readingDtos = new List<GasReadingDto>();
        for (int i = 0; i < ordered.Count; i++)
        {
            var current = ordered[i];
            decimal? deltaM3 = null;
            decimal? cost = null;

            if (i > 0)
            {
                var prev = ordered[i - 1];
                deltaM3 = Math.Max(0m, current.ReadingM3 - prev.ReadingM3);
                var rate = snapshotMap.TryGetValue((current.RecordedDate.Year, current.RecordedDate.Month), out var snap)
                    ? snap.TotalPricePerM3
                    : settings.TotalPricePerM3;
                cost = Math.Round(deltaM3.Value * rate, 2);
            }

            readingDtos.Add(new GasReadingDto(
                current.Id.Value,
                current.RecordedDate,
                current.ReadingM3,
                deltaM3,
                cost,
                current.Notes,
                current.CreatedAt));
        }

        readingDtos.Reverse();
        var recentReadings = readingDtos.Take(15).ToList();

        return new GasDashboardSummaryDto(
            Math.Round(currentMonthActualM3, 1),
            Math.Round(currentMonthProjection.TotalCostDkk, 2),
            dailyAverageM3,
            settings.TotalPricePerM3,
            settings.CommodityPricePerM3,
            settings.DistributionTaxPerM3,
            settings.MonthlySubscription,
            settings.LastSyncedAt,
            monthlySeries,
            recentReadings);
    }
}
