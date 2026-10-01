using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

public record GetInsuranceOverviewQuery(int DaysAhead = 90);

public class GetInsuranceOverviewQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public GetInsuranceOverviewQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task<InsuranceOverviewDto> HandleAsync(
        GetInsuranceOverviewQuery query,
        CancellationToken cancellationToken = default)
    {
        var allPolicies = await _repository.ListAsync(cancellationToken: cancellationToken);
        var activePolicies = allPolicies.Where(p => p.Status == InsurancePolicyStatus.Active).ToList();

        var currency = activePolicies.FirstOrDefault()?.Premium.Currency ?? "USD";

        var totalMonthly = activePolicies.Sum(p => p.CalculateMonthlyCost());
        var totalAnnual = activePolicies.Sum(p => p.CalculateAnnualCost());

        var costByCategory = activePolicies
            .GroupBy(p => p.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(p => p.CalculateAnnualCost()));

        var now = DateTime.UtcNow;
        var upcomingThreshold = now.AddDays(query.DaysAhead);

        var upcomingRenewals = activePolicies
            .Where(p => p.RenewalDate.HasValue && p.RenewalDate.Value >= now && p.RenewalDate.Value <= upcomingThreshold)
            .OrderBy(p => p.RenewalDate!.Value)
            .Select(p => new InsuranceRenewalAlertDto(
                p.Id.Value,
                p.Insurer,
                p.PolicyNumber,
                p.Category.ToString(),
                p.InsuredParty,
                p.RenewalDate!.Value,
                (int)Math.Ceiling((p.RenewalDate!.Value - now).TotalDays),
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
            upcomingRenewals);
    }
}
