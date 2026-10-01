namespace FamilyManagement.Application.DTOs.Insurances;

public record InsuranceOverviewDto(
    decimal TotalMonthlyCost,
    decimal TotalAnnualCost,
    string Currency,
    int TotalPolicies,
    int ActivePolicies,
    Dictionary<string, decimal> CostByCategory,
    List<InsuranceRenewalAlertDto> UpcomingRenewals);
