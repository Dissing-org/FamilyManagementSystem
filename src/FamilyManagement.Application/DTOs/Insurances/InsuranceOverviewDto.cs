namespace FamilyManagement.Application.DTOs.Insurances;

public record CurrencyTotalDto(
    decimal TotalMonthlyCost,
    decimal TotalAnnualCost);

public record InsuranceOverviewDto(
    decimal TotalMonthlyCost,
    decimal TotalAnnualCost,
    string Currency,
    int TotalPolicies,
    int ActivePolicies,
    Dictionary<string, decimal> CostByCategory,
    List<InsuranceRenewalAlertDto> UpcomingRenewals,
    Dictionary<string, CurrencyTotalDto>? TotalsByCurrency = null);
