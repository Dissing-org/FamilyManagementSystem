using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class MileageLogConfiguration : IEntityTypeConfiguration<MileageLogEntry>
{
    public void Configure(EntityTypeBuilder<MileageLogEntry> builder)
    {
        builder.ToTable("MileageLogs");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, value => MileageLogId.From(value))
            .IsRequired();

        builder.Property(m => m.VehicleId)
            .HasConversion(id => id.Value, value => VehicleId.From(value))
            .IsRequired();

        builder.Property(m => m.RecordedDate)
            .IsRequired();

        builder.Property(m => m.MileageKm)
            .IsRequired();

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.HasIndex(m => m.VehicleId);
        builder.HasIndex(m => new { m.VehicleId, m.RecordedDate });
    }
}
