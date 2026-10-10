using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class VehicleServiceRecordConfiguration : IEntityTypeConfiguration<VehicleServiceRecord>
{
    public void Configure(EntityTypeBuilder<VehicleServiceRecord> builder)
    {
        builder.ToTable("VehicleServiceRecords");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => ServiceRecordId.From(value))
            .IsRequired();

        builder.Property(s => s.VehicleId)
            .HasConversion(id => id.Value, value => VehicleId.From(value))
            .IsRequired();

        builder.Property(s => s.ServiceDate)
            .IsRequired();

        builder.Property(s => s.MileageKm)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Workshop)
            .HasMaxLength(200);

        builder.Property(s => s.Cost)
            .HasPrecision(18, 2);

        builder.Property(s => s.Notes)
            .HasMaxLength(2000);

        builder.Property(s => s.ReceiptId);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.VehicleId);
        builder.HasIndex(s => new { s.VehicleId, s.ServiceDate });
    }
}
