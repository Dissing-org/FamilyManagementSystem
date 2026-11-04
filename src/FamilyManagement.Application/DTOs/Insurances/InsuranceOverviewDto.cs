namespace FamilyManagement.Application.DTOs.Insurances;

/// <summary>
/// Represents aggregated monthly and annual insurance costs for a specific currency.
/// </summary>
public record CurrencyTotalDto(
    decimal TotalMonthlyCost,
    decimal TotalAnnualCost);

/// <summary>
/// Represents a high-level overview of insurance policies, including costs, policy counts, category breakdown, and upcoming renewals.
/// </summary>
public record InsuranceOverviewDto(
    decimal TotalMonthlyCost,
    decimal TotalAnnualCost,
    string Currency,
    int TotalPolicies,
    int ActivePolicies,
    Dictionary<string, decimal> CostByCategory,
    List<InsuranceRenewalAlertDto> UpcomingRenewals,
    Dictionary<string, CurrencyTotalDto>? TotalsByCurrency = null);
