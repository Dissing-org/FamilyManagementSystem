using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class GrowthMeasurementConfiguration : IEntityTypeConfiguration<GrowthMeasurement>
{
    public void Configure(EntityTypeBuilder<GrowthMeasurement> builder)
    {
        builder.ToTable("GrowthMeasurements");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasConversion(id => id.Value, value => GrowthMeasurementId.From(value));

        builder.Property(g => g.ChildId)
            .HasConversion(id => id.Value, value => ChildId.From(value))
            .IsRequired();

        builder.Property(g => g.RecordedDate)
            .IsRequired();

        builder.Property(g => g.HeightCm)
            .HasColumnType("decimal(6,2)");

        builder.Property(g => g.WeightKg)
            .HasColumnType("decimal(6,3)");

        builder.Property(g => g.HeadCircumferenceCm)
            .HasColumnType("decimal(6,2)");

        builder.Property(g => g.Notes)
            .HasMaxLength(500);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt);

        builder.HasIndex(g => g.ChildId);
        builder.HasIndex(g => new { g.ChildId, g.RecordedDate });
    }
}
