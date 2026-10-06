using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class ChildMilestoneConfiguration : IEntityTypeConfiguration<ChildMilestone>
{
    public void Configure(EntityTypeBuilder<ChildMilestone> builder)
    {
        builder.ToTable("ChildMilestones");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, value => MilestoneId.From(value));

        builder.Property(m => m.ChildId)
            .HasConversion(id => id.Value, value => ChildId.From(value))
            .IsRequired();

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.Category)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.ExpectedAgeMonths);
        builder.Property(m => m.ExpectedWindowMaxMonths);
        builder.Property(m => m.IsStandardGuideline);

        builder.Property(m => m.AchievedDate);
        builder.Property(m => m.AchievedAgeMonths);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.HasIndex(m => m.ChildId);
        builder.HasIndex(m => new { m.ChildId, m.Status });
        builder.HasIndex(m => new { m.ChildId, m.ExpectedAgeMonths });
    }
}
