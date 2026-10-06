using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class ChildProfileConfiguration : IEntityTypeConfiguration<ChildProfile>
{
    public void Configure(EntityTypeBuilder<ChildProfile> builder)
    {
        builder.ToTable("ChildProfiles");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => ChildId.From(value));

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .HasMaxLength(100);

        builder.Property(c => c.DateOfBirth)
            .IsRequired();

        builder.Property(c => c.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.AvatarUrl)
            .HasMaxLength(500);

        builder.OwnsOne(c => c.CurrentSizes, sizeBuilder =>
        {
            sizeBuilder.Property(s => s.ClothesSize)
                .HasColumnName("ClothesSize")
                .HasMaxLength(50);

            sizeBuilder.Property(s => s.ShoeSize)
                .HasColumnName("ShoeSize")
                .HasMaxLength(50);

            sizeBuilder.Property(s => s.HatSize)
                .HasColumnName("HatSize")
                .HasMaxLength(50);

            sizeBuilder.Property(s => s.DiaperSize)
                .HasColumnName("DiaperSize")
                .HasMaxLength(50);
        });

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);
    }
}
