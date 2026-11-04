using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Insurances;

/// <summary>
/// Data transfer object representing an insurance policy and its calculated costs.
/// </summary>
public record InsurancePolicyResponseDto(
    Guid Id,
    string Insurer,
    string? PolicyNumber,
    string Category,
    string InsuredParty,
    decimal PremiumAmount,
    string Currency,
    string Frequency,
    decimal MonthlyCost,
    decimal AnnualCost,
    DateTime StartDate,
    DateTime? RenewalDate,
    decimal? DeductibleAmount,
    string? Notes,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    /// <summary>
    /// Creates an <see cref="InsurancePolicyResponseDto"/> from an <see cref="InsurancePolicy"/> domain entity.
    /// </summary>
    /// <param name="policy">The insurance policy domain entity to map from.</param>
    /// <returns>A populated <see cref="InsurancePolicyResponseDto"/> instance.</returns>
    public static InsurancePolicyResponseDto FromDomain(InsurancePolicy policy)
    {
        return new InsurancePolicyResponseDto(
            policy.Id.Value,
            policy.Insurer,
            policy.PolicyNumber,
            policy.Category.ToString(),
            policy.InsuredParty,
            policy.Premium.Amount,
            policy.Premium.Currency,
            policy.Frequency.ToString(),
            policy.CalculateMonthlyCost(),
            policy.CalculateAnnualCost(),
            policy.StartDate,
            policy.RenewalDate,
            policy.Deductible?.Amount,
            policy.Notes,
            policy.Status.ToString(),
            policy.CreatedAt,
            policy.UpdatedAt);
    }
}
