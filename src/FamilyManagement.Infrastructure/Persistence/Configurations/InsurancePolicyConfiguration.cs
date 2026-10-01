using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class InsurancePolicyConfiguration : IEntityTypeConfiguration<InsurancePolicy>
{
    public void Configure(EntityTypeBuilder<InsurancePolicy> builder)
    {
        builder.ToTable("InsurancePolicies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => InsurancePolicyId.From(value))
            .IsRequired();

        builder.Property(p => p.Insurer)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.PolicyNumber)
            .HasMaxLength(100);

        builder.Property(p => p.Category)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.InsuredParty)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(p => p.Premium, premiumBuilder =>
        {
            premiumBuilder.Property(m => m.Amount)
                .HasColumnName("PremiumAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            premiumBuilder.Property(m => m.Currency)
                .HasColumnName("PremiumCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.Frequency)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.RenewalDate);

        builder.OwnsOne(p => p.Deductible, deductibleBuilder =>
        {
            deductibleBuilder.Property(m => m.Amount)
                .HasColumnName("DeductibleAmount")
                .HasPrecision(18, 2);

            deductibleBuilder.Property(m => m.Currency)
                .HasColumnName("DeductibleCurrency")
                .HasMaxLength(3);
        });

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);
    }
}
