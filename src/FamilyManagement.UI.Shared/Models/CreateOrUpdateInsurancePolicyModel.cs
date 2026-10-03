namespace FamilyManagement.UI.Shared.Models;

public class CreateOrUpdateInsurancePolicyModel
{
    public Guid? Id { get; set; }
    public string Insurer { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    public string Category { get; set; } = "Home";
    public string InsuredParty { get; set; } = string.Empty;
    public decimal PremiumAmount { get; set; }
    public string Currency { get; set; } = "DKK";
    public string Frequency { get; set; } = "Monthly";
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime? RenewalDate { get; set; }
    public decimal? DeductibleAmount { get; set; }
    public string? Notes { get; set; }

    public static CreateOrUpdateInsurancePolicyModel FromViewModel(InsurancePolicyViewModel vm)
    {
        return new CreateOrUpdateInsurancePolicyModel
        {
            Id = vm.Id,
            Insurer = vm.Insurer,
            PolicyNumber = vm.PolicyNumber,
            Category = vm.Category,
            InsuredParty = vm.InsuredParty,
            PremiumAmount = vm.PremiumAmount,
            Currency = vm.Currency,
            Frequency = vm.Frequency,
            StartDate = vm.StartDate,
            RenewalDate = vm.RenewalDate,
            DeductibleAmount = vm.DeductibleAmount,
            Notes = vm.Notes
        };
    }
}
