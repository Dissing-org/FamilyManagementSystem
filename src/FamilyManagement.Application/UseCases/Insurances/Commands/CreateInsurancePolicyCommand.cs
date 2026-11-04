using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

/// <summary>
/// Command to create a new insurance policy.
/// </summary>
public record CreateInsurancePolicyCommand(
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
/// Handler for creating a new insurance policy.
/// </summary>
public class CreateInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateInsurancePolicyCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public CreateInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the creation of a new insurance policy.
    /// </summary>
    /// <param name="command">The command containing policy creation details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A data transfer object representing the newly created insurance policy.</returns>
    /// <exception cref="ArgumentException">Thrown when insurance policy data or monetary amounts are invalid.</exception>
    public async Task<InsurancePolicyResponseDto> HandleAsync(
        CreateInsurancePolicyCommand command,
        CancellationToken cancellationToken = default)
    {
        var premium = Money.Create(command.PremiumAmount, command.Currency);
        var deductible = command.DeductibleAmount.HasValue
            ? Money.Create(command.DeductibleAmount.Value, command.Currency)
            : null;

        var policy = InsurancePolicy.Create(
            command.Insurer,
            command.PolicyNumber,
            command.Category,
            command.InsuredParty,
            premium,
            command.Frequency,
            command.StartDate,
            command.RenewalDate,
            deductible,
            command.Notes);

        await _repository.AddAsync(policy, cancellationToken);

        return InsurancePolicyResponseDto.FromDomain(policy);
    }
}
