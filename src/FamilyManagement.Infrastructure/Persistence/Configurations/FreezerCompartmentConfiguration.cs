using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class FreezerCompartmentConfiguration : IEntityTypeConfiguration<FreezerCompartment>
{
    public void Configure(EntityTypeBuilder<FreezerCompartment> builder)
    {
        builder.ToTable("FreezerCompartments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => FreezerCompartmentId.From(value))
            .IsRequired();

        builder.Property(c => c.FreezerId)
            .HasConversion(id => id.Value, value => FreezerId.From(value))
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.HasIndex(c => c.FreezerId);
    }
}
