using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

public record GetInsurancePoliciesQuery(
    InsuranceCategory? Category = null,
    InsurancePolicyStatus? Status = null,
    string? Insurer = null);

public class GetInsurancePoliciesQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public GetInsurancePoliciesQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

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
