namespace FamilyManagement.UI.Shared.Models;

public class InsuranceOverviewModel
{
    public decimal TotalMonthlyCost { get; set; }
    public decimal TotalAnnualCost { get; set; }
    public string Currency { get; set; } = "DKK";
    public int TotalPolicies { get; set; }
    public int ActivePolicies { get; set; }
    public Dictionary<string, decimal> CostByCategory { get; set; } = new();
    public List<InsuranceRenewalAlertModel> UpcomingRenewals { get; set; } = new();
}

public class InsuranceRenewalAlertModel
{
    public Guid Id { get; set; }
    public string Insurer { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    public string Category { get; set; } = string.Empty;
    public string InsuredParty { get; set; } = string.Empty;
    public DateTime RenewalDate { get; set; }
    public int DaysUntilRenewal { get; set; }
    public decimal EstimatedRenewalAmount { get; set; }
    public string Currency { get; set; } = "DKK";

    public int DaysRemaining => DaysUntilRenewal;
    public bool IsUrgent => DaysUntilRenewal <= 14;
}
