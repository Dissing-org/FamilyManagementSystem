using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IInsurancePolicyRepository
{
    Task AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);
    Task<InsurancePolicy?> GetByIdAsync(InsurancePolicyId id, CancellationToken cancellationToken = default);
    Task<List<InsurancePolicy>> ListAsync(
        InsuranceCategory? category = null,
        InsurancePolicyStatus? status = null,
        string? insurer = null,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);
    Task DeleteAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);
}
