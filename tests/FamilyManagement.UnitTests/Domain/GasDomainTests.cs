using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Services;
using FluentAssertions;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class GasDomainTests
{
    [Fact]
    public void CreateGasReading_WithValidParameters_ShouldSucceed()
    {
        var date = DateTime.UtcNow.AddDays(-1);
        var reading = GasReading.Create(date, 14250.5m, "Start reading");

        reading.Should().NotBeNull();
        reading.ReadingM3.Should().Be(14250.5m);
        reading.Notes.Should().Be("Start reading");
        reading.RecordedDate.Should().Be(date);
    }

    [Fact]
    public void CreateGasReading_WithNegativeReading_ShouldThrowArgumentException()
    {
        var act = () => GasReading.Create(DateTime.UtcNow, -1m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void CreateGasReading_WithFutureDate_ShouldThrowArgumentException()
    {
        var act = () => GasReading.Create(DateTime.UtcNow.AddDays(2), 100m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void UpdateGasReading_ShouldModifyValuesAndSetUpdatedAt()
    {
        var reading = GasReading.Create(DateTime.UtcNow.AddDays(-5), 100m, "Initial");
        var newDate = DateTime.UtcNow.AddDays(-2);

        reading.Update(newDate, 150m, "Updated notes");

        reading.ReadingM3.Should().Be(150m);
        reading.Notes.Should().Be("Updated notes");
        reading.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void GasPricingSettings_CalculatesTotalPricePerM3Accurately()
    {
        var settings = new GasPricingSettings();
        settings.Update(10.63m, 4.45m, 12.50m);

        settings.CommodityPricePerM3.Should().Be(10.63m);
        settings.DistributionTaxPerM3.Should().Be(4.45m);
        settings.MonthlySubscription.Should().Be(12.50m);
        settings.TotalPricePerM3.Should().Be(15.08m);
    }

    [Fact]
    public void GasConsumptionCalculator_CalculateDeltas_WithLessThanTwoReadings_ReturnsEmpty()
    {
        var single = new List<GasReading> { GasReading.Create(DateTime.UtcNow, 100m) };
        var deltas = GasConsumptionCalculator.CalculateDeltas(single);
        deltas.Should().BeEmpty();
    }

    [Fact]
    public void GasConsumptionCalculator_CalculateDeltas_CalculatesDeltaAndDailyBurn()
    {
        var r1 = GasReading.Create(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1000m);
        var r2 = GasReading.Create(new DateTime(2026, 1, 11, 0, 0, 0, DateTimeKind.Utc), 1100m);

        var deltas = GasConsumptionCalculator.CalculateDeltas(new[] { r1, r2 });

        deltas.Should().HaveCount(1);
        deltas[0].DeltaM3.Should().Be(100m);
        deltas[0].DaysSpan.Should().Be(10);
        deltas[0].DailyBurnM3.Should().Be(10m);
    }

    [Fact]
    public void GasConsumptionCalculator_ApportionMonthlyConsumption_SpansAcrossMonthBoundaries()
    {
        // 60 m3 across 20 days: 10 days in Jan (Jan 22 to Feb 1) and 10 days in Feb (Feb 1 to Feb 11)
        var r1 = GasReading.Create(new DateTime(2026, 1, 22, 0, 0, 0, DateTimeKind.Utc), 1000m);
        var r2 = GasReading.Create(new DateTime(2026, 2, 11, 0, 0, 0, DateTimeKind.Utc), 1060m);

        var map = GasConsumptionCalculator.ApportionMonthlyConsumption(new[] { r1, r2 });

        // Total 20 days: 10 days in Jan, 10 days in Feb => 3 m3/day
        map.Should().ContainKey((2026, 1));
        map.Should().ContainKey((2026, 2));
        map[(2026, 1)].Should().Be(30m);
        map[(2026, 2)].Should().Be(30m);
    }

    [Fact]
    public void GasConsumptionCalculator_CalculateProjectedCurrentMonth_UsesRunRate()
    {
        var settings = new GasPricingSettings();
        settings.Update(10m, 5m, 10m); // Total 15 kr./m3 + 10 kr. sub

        // Suppose 10 days into month with 30 m3 used -> daily avg = 3 m3/day
        var date = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc); // April has 30 days
        var projection = GasConsumptionCalculator.CalculateProjectedCurrentMonth(30m, date, settings);

        // Daily avg = 30 / 10 = 3 m3/day. 30 days in April => 90 m3 total projected
        projection.ConsumedM3.Should().Be(90m);
        // Cost = (90 * 15) + 10 = 1350 + 10 = 1360 kr.
        projection.TotalCostDkk.Should().Be(1360m);
        projection.IsProjected.Should().BeTrue();
    }
}
