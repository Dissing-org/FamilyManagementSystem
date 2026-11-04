using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

/// <summary>
/// Command to update the details and premium of an existing insurance policy.
/// </summary>
public record UpdateInsurancePolicyCommand(
    Guid Id,
    string Insurer,
    string? PolicyNumber,
    InsuranceCategory Category,
    string InsuredParty,
    decimal PremiumAmount,
    string Currency,
    PaymentFrequency Frequency,
    DateTime StartDate,
    DateTime? RenewalDate = null,
    decimal? DeductibleAmount = null,
    string? Notes = null);

/// <summary>
/// Handler for updating an existing insurance policy.
/// </summary>
public class UpdateInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInsurancePolicyCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public UpdateInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles updating an existing insurance policy.
    /// </summary>
    /// <param name="command">The command containing updated policy details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the updated insurance policy.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the insurance policy with the specified identifier is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when policy data or monetary amounts are invalid.</exception>
    public async Task<InsurancePolicyResponseDto> HandleAsync(
        UpdateInsurancePolicyCommand command,
        CancellationToken cancellationToken = default)
    {
        var policy = await _repository.GetByIdAsync(InsurancePolicyId.From(command.Id), cancellationToken);
        if (policy is null)
        {
            throw new KeyNotFoundException($"Insurance policy with ID '{command.Id}' was not found.");
        }

        var deductible = command.DeductibleAmount.HasValue
            ? Money.Create(command.DeductibleAmount.Value, command.Currency)
            : null;

        policy.UpdateDetails(
            command.Insurer,
            command.PolicyNumber,
            command.Category,
            command.InsuredParty,
            command.StartDate,
            command.RenewalDate,
            deductible,
            command.Notes);

        var premium = Money.Create(command.PremiumAmount, command.Currency);
        policy.UpdatePremium(premium, command.Frequency);

        await _repository.UpdateAsync(policy, cancellationToken);

        return InsurancePolicyResponseDto.FromDomain(policy);
    }
}
