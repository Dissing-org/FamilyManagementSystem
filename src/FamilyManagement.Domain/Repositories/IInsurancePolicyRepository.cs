using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="InsurancePolicy"/> entities.
/// </summary>
public interface IInsurancePolicyRepository
{
    /// <summary>
    /// Persists a new insurance policy to the repository.
    /// </summary>
    /// <param name="policy">The insurance policy entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an insurance policy by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the insurance policy.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching insurance policy or <c>null</c> if not found.</returns>
    Task<InsurancePolicy?> GetByIdAsync(InsurancePolicyId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves insurance policies matching optional filter criteria.
    /// </summary>
    /// <param name="category">An optional insurance category filter.</param>
    /// <param name="status">An optional policy status filter.</param>
    /// <param name="insurer">An optional insurer name filter.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of matching insurance policies.</returns>
    Task<List<InsurancePolicy>> ListAsync(
        InsuranceCategory? category = null,
        InsurancePolicyStatus? status = null,
        string? insurer = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing insurance policy in the repository.
    /// </summary>
    /// <param name="policy">The insurance policy entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an insurance policy from the repository.
    /// </summary>
    /// <param name="policy">The insurance policy entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);
}
