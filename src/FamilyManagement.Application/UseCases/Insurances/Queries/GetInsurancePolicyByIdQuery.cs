using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

public record GetInsurancePolicyByIdQuery(Guid Id);

public class GetInsurancePolicyByIdQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public GetInsurancePolicyByIdQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task<InsurancePolicyResponseDto?> HandleAsync(
        GetInsurancePolicyByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var policy = await _repository.GetByIdAsync(InsurancePolicyId.From(query.Id), cancellationToken);
        return policy is not null ? InsurancePolicyResponseDto.FromDomain(policy) : null;
    }
}
