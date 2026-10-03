namespace FamilyManagement.UI.Shared.Models;

public class InsurancePolicyViewModel
{
    public Guid Id { get; set; }
    public string Insurer { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    public string Category { get; set; } = "Other";
    public string InsuredParty { get; set; } = string.Empty;
    public decimal PremiumAmount { get; set; }
    public string Currency { get; set; } = "DKK";
    public string Frequency { get; set; } = "Monthly";
    public decimal MonthlyCost { get; set; }
    public decimal AnnualCost { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public decimal? DeductibleAmount { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
