using FamilyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class GasPricingSettingsConfiguration : IEntityTypeConfiguration<GasPricingSettings>
{
    public void Configure(EntityTypeBuilder<GasPricingSettings> builder)
    {
        builder.ToTable("GasPricingSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CommodityPricePerM3).HasPrecision(18, 4).IsRequired();
        builder.Property(s => s.DistributionTaxPerM3).HasPrecision(18, 4).IsRequired();
        builder.Property(s => s.MonthlySubscription).HasPrecision(18, 2).IsRequired();
    }
}
