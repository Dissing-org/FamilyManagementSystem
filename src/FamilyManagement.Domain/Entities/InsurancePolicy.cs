using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class InsurancePolicy
{
    public InsurancePolicyId Id { get; private set; }
    public string Insurer { get; private set; }
    public string? PolicyNumber { get; private set; }
    public InsuranceCategory Category { get; private set; }
    public string InsuredParty { get; private set; }
    public Money Premium { get; private set; }
    public PaymentFrequency Frequency { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? RenewalDate { get; private set; }
    public Money? Deductible { get; private set; }
    public string? Notes { get; private set; }
    public InsurancePolicyStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core parameterless constructor
    private InsurancePolicy()
    {
        Id = null!;
        Insurer = null!;
        InsuredParty = null!;
        Premium = null!;
    }

    private InsurancePolicy(
        InsurancePolicyId id,
        string insurer,
        string? policyNumber,
        InsuranceCategory category,
        string insuredParty,
        Money premium,
        PaymentFrequency frequency,
        DateTime startDate,
        DateTime? renewalDate,
        Money? deductible,
        string? notes,
        InsurancePolicyStatus status,
        DateTime createdAt)
    {
        Id = id;
        Insurer = insurer;
        PolicyNumber = policyNumber;
        Category = category;
        InsuredParty = insuredParty;
        Premium = premium;
        Frequency = frequency;
        StartDate = startDate;
        RenewalDate = renewalDate;
        Deductible = deductible;
        Notes = notes;
        Status = status;
        CreatedAt = createdAt;
    }

    public static InsurancePolicy Create(
        string insurer,
        string? policyNumber,
        InsuranceCategory category,
        string insuredParty,
        Money premium,
        PaymentFrequency frequency,
        DateTime startDate,
        DateTime? renewalDate = null,
        Money? deductible = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(insurer))
        {
            throw new ArgumentException("Insurer is required.", nameof(insurer));
        }

        if (string.IsNullOrWhiteSpace(insuredParty))
        {
            throw new ArgumentException("Insured party or asset is required.", nameof(insuredParty));
        }

        if (premium is null || premium.Amount <= 0)
        {
            throw new ArgumentException("Premium amount must be greater than zero.", nameof(premium));
        }

        return new InsurancePolicy(
            InsurancePolicyId.New(),
            insurer.Trim(),
            policyNumber?.Trim(),
            category,
            insuredParty.Trim(),
            premium,
            frequency,
            DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
            renewalDate.HasValue ? DateTime.SpecifyKind(renewalDate.Value, DateTimeKind.Utc) : null,
            deductible,
            notes?.Trim(),
            InsurancePolicyStatus.Active,
            DateTime.UtcNow);
    }

    public void UpdateDetails(
        string insurer,
        string? policyNumber,
        InsuranceCategory category,
        string insuredParty,
        DateTime startDate,
        DateTime? renewalDate,
        Money? deductible,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(insurer))
        {
            throw new ArgumentException("Insurer is required.", nameof(insurer));
        }

        if (string.IsNullOrWhiteSpace(insuredParty))
        {
            throw new ArgumentException("Insured party or asset is required.", nameof(insuredParty));
        }

        if (renewalDate.HasValue && renewalDate.Value < startDate)
        {
            throw new ArgumentException("Renewal date cannot be earlier than start date.", nameof(renewalDate));
        }

        Insurer = insurer.Trim();
        PolicyNumber = policyNumber?.Trim();
        Category = category;
        InsuredParty = insuredParty.Trim();
        StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        RenewalDate = renewalDate.HasValue ? DateTime.SpecifyKind(renewalDate.Value, DateTimeKind.Utc) : null;
        Deductible = deductible;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePremium(Money newPremium, PaymentFrequency newFrequency)
    {
        if (newPremium is null || newPremium.Amount <= 0)
        {
            throw new ArgumentException("Premium amount must be greater than zero.", nameof(newPremium));
        }

        Premium = newPremium;
        Frequency = newFrequency;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = InsurancePolicyStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = InsurancePolicyStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        Status = InsurancePolicyStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal CalculateMonthlyCost()
    {
        return Frequency switch
        {
            PaymentFrequency.Monthly => Premium.Amount,
            PaymentFrequency.Quarterly => Math.Round(Premium.Amount / 3m, 2),
            PaymentFrequency.SemiAnnually => Math.Round(Premium.Amount / 6m, 2),
            PaymentFrequency.Annually => Math.Round(Premium.Amount / 12m, 2),
            _ => Premium.Amount
        };
    }

    public decimal CalculateAnnualCost()
    {
        return Frequency switch
        {
            PaymentFrequency.Monthly => Premium.Amount * 12m,
            PaymentFrequency.Quarterly => Premium.Amount * 4m,
            PaymentFrequency.SemiAnnually => Premium.Amount * 2m,
            PaymentFrequency.Annually => Premium.Amount,
            _ => Premium.Amount
        };
    }
}
