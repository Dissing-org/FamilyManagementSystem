using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

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

public class CreateInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public CreateInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

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
