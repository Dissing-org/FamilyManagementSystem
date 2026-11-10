using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Domain.Services;

public record MonthConsumption(
    int Year,
    int Month,
    decimal ConsumedM3,
    decimal TotalCostDkk,
    decimal EffectivePricePerM3,
    bool IsProjected);

public record PeriodReadingDelta(
    GasReading StartReading,
    GasReading EndReading,
    decimal DeltaM3,
    int DaysSpan,
    decimal DailyBurnM3);

public static class GasConsumptionCalculator
{
    public static List<PeriodReadingDelta> CalculateDeltas(IReadOnlyList<GasReading> readings)
    {
        if (readings == null || readings.Count < 2)
        {
            return new List<PeriodReadingDelta>();
        }

        var ordered = readings.OrderBy(r => r.RecordedDate).ToList();
        var deltas = new List<PeriodReadingDelta>();

        for (int i = 0; i < ordered.Count - 1; i++)
        {
            var prev = ordered[i];
            var next = ordered[i + 1];
            var deltaM3 = Math.Max(0m, next.ReadingM3 - prev.ReadingM3);
            var days = (int)Math.Max(1, Math.Round((next.RecordedDate.Date - prev.RecordedDate.Date).TotalDays));
            var dailyBurn = days > 0 ? (deltaM3 / days) : 0m;

            deltas.Add(new PeriodReadingDelta(prev, next, deltaM3, days, dailyBurn));
        }

        return deltas;
    }

    public static Dictionary<(int Year, int Month), decimal> ApportionMonthlyConsumption(IReadOnlyList<GasReading> readings)
    {
        var monthlyMap = new Dictionary<(int Year, int Month), decimal>();
        var deltas = CalculateDeltas(readings);

        foreach (var delta in deltas)
        {
            var curDate = delta.StartReading.RecordedDate.Date;
            var endDate = delta.EndReading.RecordedDate.Date;
            var dailyRate = delta.DailyBurnM3;

            while (curDate < endDate)
            {
                var key = (curDate.Year, curDate.Month);
                monthlyMap[key] = monthlyMap.GetValueOrDefault(key) + dailyRate;
                curDate = curDate.AddDays(1);
            }
        }

        return monthlyMap;
    }

    public static MonthConsumption CalculateProjectedCurrentMonth(
        decimal monthToDateM3,
        DateTime now,
        GasPricingSettings settings)
    {
        var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
        var daysElapsed = Math.Max(1, now.Day);
        var dailyAvg = monthToDateM3 / daysElapsed;
        var projectedM3 = Math.Round(dailyAvg * daysInMonth, 2);
        var totalCost = Math.Round((projectedM3 * settings.TotalPricePerM3) + settings.MonthlySubscription, 2);

        return new MonthConsumption(
            now.Year,
            now.Month,
            projectedM3,
            totalCost,
            settings.TotalPricePerM3,
            IsProjected: true);
    }
}
