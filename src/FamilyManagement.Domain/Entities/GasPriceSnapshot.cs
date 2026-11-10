using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class GasPriceSnapshot
{
    public GasPriceSnapshotId Id { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public decimal CommodityPricePerM3 { get; private set; }
    public decimal DistributionTaxPerM3 { get; private set; }
    public decimal MonthlySubscription { get; private set; }
    public DateTime FetchedAt { get; private set; }
    public bool IsManuallyOverridden { get; private set; }

    public decimal TotalPricePerM3 => CommodityPricePerM3 + DistributionTaxPerM3;

    private GasPriceSnapshot()
    {
        Id = null!;
    }

    public GasPriceSnapshot(
        GasPriceSnapshotId id,
        int year,
        int month,
        decimal commodityPricePerM3,
        decimal distributionTaxPerM3,
        decimal monthlySubscription,
        DateTime fetchedAt,
        bool isManuallyOverridden)
    {
        if (year < 2000 || year > 2100) throw new ArgumentOutOfRangeException(nameof(year));
        if (month < 1 || month > 12) throw new ArgumentOutOfRangeException(nameof(month));
        if (commodityPricePerM3 < 0) throw new ArgumentException("Price cannot be negative.", nameof(commodityPricePerM3));
        if (distributionTaxPerM3 < 0) throw new ArgumentException("Tax cannot be negative.", nameof(distributionTaxPerM3));
        if (monthlySubscription < 0) throw new ArgumentException("Subscription cannot be negative.", nameof(monthlySubscription));

        Id = id;
        Year = year;
        Month = month;
        CommodityPricePerM3 = commodityPricePerM3;
        DistributionTaxPerM3 = distributionTaxPerM3;
        MonthlySubscription = monthlySubscription;
        FetchedAt = fetchedAt;
        IsManuallyOverridden = isManuallyOverridden;
    }

    public static GasPriceSnapshot Create(
        int year,
        int month,
        decimal commodityPricePerM3,
        decimal distributionTaxPerM3,
        decimal monthlySubscription,
        bool isManuallyOverridden = false)
    {
        return new GasPriceSnapshot(
            GasPriceSnapshotId.New(),
            year,
            month,
            commodityPricePerM3,
            distributionTaxPerM3,
            monthlySubscription,
            DateTime.UtcNow,
            isManuallyOverridden);
    }
}
