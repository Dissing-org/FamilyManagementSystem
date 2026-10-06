using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

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

public class UpdateInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public UpdateInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

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
