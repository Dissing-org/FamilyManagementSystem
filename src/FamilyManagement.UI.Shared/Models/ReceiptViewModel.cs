namespace FamilyManagement.UI.Shared.Models;

public class ReceiptViewModel
{
    public Guid Id { get; set; }
    public string Merchant { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Category { get; set; } = "Other";
    public string GoogleDriveFileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? WebViewLink { get; set; }
    public string? Notes { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public string FormattedAmount => $"{Amount:N2} {Currency}";
    public string FormattedDate => PurchaseDate.ToString("MMM dd, yyyy");
}

public class UploadReceiptModel
{
    public string Merchant { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.Today;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Category { get; set; } = ReceiptCategories.Groceries;
    public string? Notes { get; set; }

    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public byte[]? FileBytes { get; set; }
}
