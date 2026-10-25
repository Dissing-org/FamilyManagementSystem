using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class FreezerCatalogItemConfiguration : IEntityTypeConfiguration<FreezerCatalogItem>
{
    public void Configure(EntityTypeBuilder<FreezerCatalogItem> builder)
    {
        builder.ToTable("FreezerCatalogItems");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => FreezerCatalogItemId.From(value))
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.DefaultCategory)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.DefaultUnit)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.DefaultMinimumQuantity)
            .HasPrecision(18, 2);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.LastUsedAt)
            .IsRequired();
    }
}
