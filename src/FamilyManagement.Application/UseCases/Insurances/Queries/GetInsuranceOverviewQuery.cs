using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

/// <summary>
/// Query to retrieve an aggregated overview of insurance policies, costs, and upcoming renewals.
/// </summary>
public record GetInsuranceOverviewQuery(int DaysAhead = 90);

/// <summary>
/// Handler for retrieving the insurance overview.
/// </summary>
public class GetInsuranceOverviewQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsuranceOverviewQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public GetInsuranceOverviewQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving an overview of insurance policies, costs, and upcoming renewals.
    /// </summary>
    /// <param name="query">The query containing parameters for the overview calculation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="InsuranceOverviewDto"/> containing aggregated insurance metrics and upcoming renewals.</returns>
    public async Task<InsuranceOverviewDto> HandleAsync(
        GetInsuranceOverviewQuery query,
        CancellationToken cancellationToken = default)
    {
        var allPolicies = await _repository.ListAsync(cancellationToken: cancellationToken);
        var activePolicies = allPolicies.Where(p => p.Status == InsurancePolicyStatus.Active).ToList();

        var totalsByCurrency = activePolicies
            .GroupBy(p => p.Premium.Currency)
            .ToDictionary(
                g => g.Key,
                g => new CurrencyTotalDto(
                    g.Sum(p => p.CalculateMonthlyCost()),
                    g.Sum(p => p.CalculateAnnualCost())));

        var primaryGroup = activePolicies
            .GroupBy(p => p.Premium.Currency)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        var currency = primaryGroup?.Key ?? "DKK";
        var totalMonthly = primaryGroup != null ? totalsByCurrency[currency].TotalMonthlyCost : 0m;
        var totalAnnual = primaryGroup != null ? totalsByCurrency[currency].TotalAnnualCost : 0m;

        var costByCategory = activePolicies
            .Where(p => p.Premium.Currency == currency)
            .GroupBy(p => p.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(p => p.CalculateAnnualCost()));

        var now = DateTime.UtcNow;
        var upcomingThreshold = now.AddDays(query.DaysAhead);

        var upcomingRenewals = activePolicies
            .Where(p => p.RenewalDate.HasValue && p.RenewalDate.Value.Date >= now.Date && p.RenewalDate.Value.Date <= upcomingThreshold.Date)
            .OrderBy(p => p.RenewalDate!.Value)
            .Select(p => new InsuranceRenewalAlertDto(
                p.Id.Value,
                p.Insurer,
                p.PolicyNumber,
                p.Category.ToString(),
                p.InsuredParty,
                p.RenewalDate!.Value,
                (int)Math.Ceiling((p.RenewalDate!.Value.Date - now.Date).TotalDays),
                p.Premium.Amount,
                p.Premium.Currency))
            .ToList();

        return new InsuranceOverviewDto(
            totalMonthly,
            totalAnnual,
            currency,
            allPolicies.Count,
            activePolicies.Count,
            costByCategory,
            upcomingRenewals,
            totalsByCurrency);
    }
}
