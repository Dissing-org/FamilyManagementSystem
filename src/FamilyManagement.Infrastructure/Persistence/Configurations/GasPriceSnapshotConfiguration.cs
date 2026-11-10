using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class GasPriceSnapshotConfiguration : IEntityTypeConfiguration<GasPriceSnapshot>
{
    public void Configure(EntityTypeBuilder<GasPriceSnapshot> builder)
    {
        builder.ToTable("GasPriceSnapshots");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value.ToString(), val => GasPriceSnapshotId.From(Guid.Parse(val)))
            .IsRequired();

        builder.Property(s => s.Year).IsRequired();
        builder.Property(s => s.Month).IsRequired();
        builder.Property(s => s.CommodityPricePerM3).HasPrecision(18, 4).IsRequired();
        builder.Property(s => s.DistributionTaxPerM3).HasPrecision(18, 4).IsRequired();
        builder.Property(s => s.MonthlySubscription).HasPrecision(18, 2).IsRequired();

        builder.HasIndex(s => new { s.Year, s.Month }).IsUnique();
    }
}
