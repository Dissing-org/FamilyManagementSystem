using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class FreezerConfiguration : IEntityTypeConfiguration<Freezer>
{
    public void Configure(EntityTypeBuilder<Freezer> builder)
    {
        builder.ToTable("Freezers");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, value => FreezerId.From(value))
            .IsRequired();

        builder.Property(f => f.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.Location)
            .HasMaxLength(100);

        builder.Property(f => f.DisplayOrder)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.HasMany(f => f.Compartments)
            .WithOne()
            .HasForeignKey(c => c.FreezerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(f => f.Compartments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
