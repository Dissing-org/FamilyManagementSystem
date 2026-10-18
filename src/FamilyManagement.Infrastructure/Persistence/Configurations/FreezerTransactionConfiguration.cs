using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class FreezerTransactionConfiguration : IEntityTypeConfiguration<FreezerTransaction>
{
    public void Configure(EntityTypeBuilder<FreezerTransaction> builder)
    {
        builder.ToTable("FreezerTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => FreezerTransactionId.From(value))
            .IsRequired();

        builder.Property(t => t.ItemId)
            .HasConversion(id => id.Value, value => FreezerItemId.From(value))
            .IsRequired();

        builder.Property(t => t.QuantityChanged)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.RemainingQuantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.Reason)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.Property(t => t.Timestamp)
            .IsRequired();

        builder.HasIndex(t => t.ItemId);
        builder.HasIndex(t => t.Timestamp);
    }
}
