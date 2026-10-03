using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyManagement.Infrastructure.Persistence.Configurations;

public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> builder)
    {
        builder.ToTable("Receipts");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => ReceiptId.From(value))
            .IsRequired();

        builder.Property(r => r.Merchant)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.PurchaseDate)
            .IsRequired();

        builder.OwnsOne(r => r.FileReference, fileBuilder =>
        {
            fileBuilder.Property(f => f.FileId)
                .HasColumnName("GoogleDriveFileId")
                .HasMaxLength(250)
                .IsRequired();

            fileBuilder.Property(f => f.FileName)
                .HasColumnName("FileName")
                .HasMaxLength(250)
                .IsRequired();

            fileBuilder.Property(f => f.WebViewLink)
                .HasColumnName("WebViewLink")
                .HasMaxLength(500);
        });

        builder.Property(r => r.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ArchivedAt);
    }
}
