using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Queries;

/// <summary>
/// Query to retrieve an insurance policy by its unique identifier.
/// </summary>
public record GetInsurancePolicyByIdQuery(Guid Id);

/// <summary>
/// Handler for retrieving an insurance policy by its unique identifier.
/// </summary>
public class GetInsurancePolicyByIdQueryHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsurancePolicyByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public GetInsurancePolicyByIdQueryHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving an insurance policy by its unique identifier.
    /// </summary>
    /// <param name="query">The query containing the policy identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="InsurancePolicyResponseDto"/> if found; otherwise, <see langword="null"/>.</returns>
    public async Task<InsurancePolicyResponseDto?> HandleAsync(
        GetInsurancePolicyByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var policy = await _repository.GetByIdAsync(InsurancePolicyId.From(query.Id), cancellationToken);
        return policy is not null ? InsurancePolicyResponseDto.FromDomain(policy) : null;
    }
}
