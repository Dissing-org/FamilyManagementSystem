using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class FreezerItemConfiguration : IEntityTypeConfiguration<FreezerItem>
{
    public void Configure(EntityTypeBuilder<FreezerItem> builder)
    {
        builder.ToTable("FreezerItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(id => id.Value, value => FreezerItemId.From(value))
            .IsRequired();

        builder.Property(i => i.CompartmentId)
            .HasConversion(id => id.Value, value => FreezerCompartmentId.From(value))
            .IsRequired();

        builder.Property(i => i.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(i => i.Category)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Unit)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.FreezeDate)
            .IsRequired();

        builder.Property(i => i.ExpiryDate);

        builder.Property(i => i.MinimumQuantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        builder.HasIndex(i => i.CompartmentId);
        builder.HasIndex(i => i.Category);
        builder.HasIndex(i => i.ExpiryDate);
    }
}
