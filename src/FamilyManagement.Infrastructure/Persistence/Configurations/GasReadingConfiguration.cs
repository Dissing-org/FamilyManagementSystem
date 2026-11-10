using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class GasReadingConfiguration : IEntityTypeConfiguration<GasReading>
{
    public void Configure(EntityTypeBuilder<GasReading> builder)
    {
        builder.ToTable("GasReadings");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(id => id.Value.ToString(), val => GasReadingId.From(Guid.Parse(val)))
            .IsRequired();

        builder.Property(r => r.ReadingM3).HasPrecision(18, 3).IsRequired();
        builder.Property(r => r.RecordedDate).IsRequired();
        builder.Property(r => r.Notes).HasMaxLength(500);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasIndex(r => r.RecordedDate);
    }
}
