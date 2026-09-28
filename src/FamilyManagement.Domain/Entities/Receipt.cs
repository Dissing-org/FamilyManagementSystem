using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class Receipt
{
    public ReceiptId Id { get; private set; }
    public string Merchant { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public Money Amount { get; private set; }
    public GoogleDriveFileReference FileReference { get; private set; }
    public string Category { get; private set; }
    public string? Notes { get; private set; }
    public ReceiptStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }

    // Required by EF Core
    private Receipt() 
    {
        Id = null!;
        Merchant = null!;
        Amount = null!;
        FileReference = null!;
        Category = "Other";
    }

    private Receipt(
        ReceiptId id,
        string merchant,
        DateTime purchaseDate,
        Money amount,
        GoogleDriveFileReference fileReference,
        string category,
        string? notes,
        ReceiptStatus status,
        DateTime createdAt)
    {
        Id = id;
        Merchant = merchant;
        PurchaseDate = purchaseDate;
        Amount = amount;
        FileReference = fileReference;
        Category = category;
        Notes = notes;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Receipt Create(
        string merchant,
        DateTime purchaseDate,
        Money amount,
        GoogleDriveFileReference fileReference,
        string? category = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(merchant))
        {
            throw new ArgumentException("Merchant is required.", nameof(merchant));
        }

        var utcPurchaseDate = purchaseDate.Kind switch
        {
            DateTimeKind.Utc => purchaseDate,
            DateTimeKind.Local => purchaseDate.ToUniversalTime(),
            _ => DateTime.SpecifyKind(purchaseDate, DateTimeKind.Utc)
        };

        if (utcPurchaseDate > DateTime.UtcNow.AddMinutes(5)) // small leeway for clock drift
        {
            throw new ArgumentException("Purchase date cannot be in the future.", nameof(purchaseDate));
        }

        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(fileReference);

        return new Receipt(
            ReceiptId.New(),
            merchant.Trim(),
            utcPurchaseDate,
            amount,
            fileReference,
            string.IsNullOrWhiteSpace(category) ? "Other" : category.Trim(),
            notes?.Trim(),
            ReceiptStatus.Active,
            DateTime.UtcNow);
    }

    public void Archive()
    {
        if (Status == ReceiptStatus.Archived)
        {
            return;
        }

        Status = ReceiptStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
    }
}
