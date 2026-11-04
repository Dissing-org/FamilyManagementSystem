using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

/// <summary>
/// Query to retrieve insurance policies, optionally filtered by category, status, or insurer.
/// </summary>
public record GetInsurancePoliciesQuery(
    InsuranceCategory? Category = null,
    InsurancePolicyStatus? Status = null,
    string? Insurer = null);

/// <summary>
/// Handler for retrieving filtered insurance policies.
/// </summary>
public class GetInsurancePoliciesQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsurancePoliciesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public GetInsurancePoliciesQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving insurance policies matching the query filters.
    /// </summary>
    /// <param name="query">The query containing filter criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="InsurancePolicyResponseDto"/> matching the filter criteria.</returns>
    public async Task<List<InsurancePolicyResponseDto>> HandleAsync(
        GetInsurancePoliciesQuery query,
        CancellationToken cancellationToken = default)
    {
        var policies = await _repository.ListAsync(
            query.Category,
            query.Status,
            query.Insurer,
            cancellationToken);

        return policies.Select(InsurancePolicyResponseDto.FromDomain).ToList();
    }
}
