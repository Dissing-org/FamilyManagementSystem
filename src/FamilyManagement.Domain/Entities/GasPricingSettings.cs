namespace FamilyManagement.Domain.Entities;

public class GasPricingSettings
{
    public string Id { get; private set; } = "default";
    public decimal CommodityPricePerM3 { get; private set; }
    public decimal DistributionTaxPerM3 { get; private set; }
    public decimal MonthlySubscription { get; private set; }
    public DateTime? LastSyncedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public decimal TotalPricePerM3 => CommodityPricePerM3 + DistributionTaxPerM3;

    public GasPricingSettings()
    {
        CommodityPricePerM3 = 10.63m;
        DistributionTaxPerM3 = 4.45m;
        MonthlySubscription = 12.50m;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(decimal commodityPrice, decimal distributionTax, decimal monthlySubscription)
    {
        if (commodityPrice < 0) throw new ArgumentException("Commodity price cannot be negative.", nameof(commodityPrice));
        if (distributionTax < 0) throw new ArgumentException("Distribution/tax cannot be negative.", nameof(distributionTax));
        if (monthlySubscription < 0) throw new ArgumentException("Subscription cannot be negative.", nameof(monthlySubscription));

        CommodityPricePerM3 = commodityPrice;
        DistributionTaxPerM3 = distributionTax;
        MonthlySubscription = monthlySubscription;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SyncOkPrice(decimal commodityPrice)
    {
        if (commodityPrice < 0) throw new ArgumentException("Commodity price cannot be negative.", nameof(commodityPrice));
        CommodityPricePerM3 = commodityPrice;
        LastSyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
