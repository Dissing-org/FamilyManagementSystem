using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => VehicleId.From(value))
            .IsRequired();

        builder.Property(v => v.Make)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.Year)
            .IsRequired();

        builder.Property(v => v.LicensePlate)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Vin)
            .HasMaxLength(50);

        builder.Property(v => v.FuelType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.CurrentMileageKm)
            .IsRequired();

        builder.Property(v => v.ServiceIntervalKm);

        builder.Property(v => v.ServiceIntervalMonths);

        builder.Property(v => v.NextInspectionDate);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt);

        builder.HasIndex(v => v.LicensePlate);
    }
}
