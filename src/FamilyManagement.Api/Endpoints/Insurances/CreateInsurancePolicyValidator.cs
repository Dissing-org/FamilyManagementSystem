using FastEndpoints;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class CreateInsurancePolicyValidator : Validator<CreateInsurancePolicyRequest>
{
    public CreateInsurancePolicyValidator()
    {
        RuleFor(x => x.Insurer)
            .NotEmpty().WithMessage("Insurer is required.")
            .MaximumLength(100).WithMessage("Insurer cannot exceed 100 characters.");

        RuleFor(x => x.PolicyNumber)
            .MaximumLength(100).WithMessage("Policy number cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.PolicyNumber));

        RuleFor(x => x.InsuredParty)
            .NotEmpty().WithMessage("Insured party or asset is required.")
            .MaximumLength(100).WithMessage("Insured party cannot exceed 100 characters.");

        RuleFor(x => x.PremiumAmount)
            .GreaterThan(0).WithMessage("Premium amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter currency code.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("A valid insurance category must be selected.");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("A valid payment frequency must be selected.");

        RuleFor(x => x.RenewalDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.RenewalDate.HasValue)
            .WithMessage("Renewal date must be greater than or equal to start date.");
    }
}
