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

    /// <summary>
    /// Creates a new <see cref="InsurancePolicy"/> instance with the specified policy details.
    /// </summary>
    /// <param name="insurer">The name of the insurance company or provider.</param>
    /// <param name="policyNumber">An optional policy identifier or contract number.</param>
    /// <param name="category">The category of the insurance policy.</param>
    /// <param name="insuredParty">The name of the insured person, vehicle, or asset.</param>
    /// <param name="premium">The recurring premium cost for the policy.</param>
    /// <param name="frequency">The payment frequency for the premium.</param>
    /// <param name="startDate">The date when coverage begins.</param>
    /// <param name="renewalDate">The optional renewal date of the policy.</param>
    /// <param name="deductible">The optional deductible amount applicable to claims.</param>
    /// <param name="notes">Optional notes or terms regarding the policy.</param>
    /// <returns>A new <see cref="InsurancePolicy"/> instance initialized with active status.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="insurer"/> or <paramref name="insuredParty"/> is null or whitespace, or when <paramref name="premium"/> is null or has an amount less than or equal to zero.</exception>
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

    /// <summary>
    /// Updates the descriptive details and validity dates of the insurance policy.
    /// </summary>
    /// <param name="insurer">The name of the insurance company or provider.</param>
    /// <param name="policyNumber">An optional policy identifier or contract number.</param>
    /// <param name="category">The category of the insurance policy.</param>
    /// <param name="insuredParty">The name of the insured person, vehicle, or asset.</param>
    /// <param name="startDate">The date when policy coverage begins.</param>
    /// <param name="renewalDate">The optional renewal date of the policy.</param>
    /// <param name="deductible">The optional deductible amount applicable to claims.</param>
    /// <param name="notes">Optional notes or terms regarding the policy.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="insurer"/> or <paramref name="insuredParty"/> is null or whitespace, or when <paramref name="renewalDate"/> is earlier than <paramref name="startDate"/>.</exception>
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

    /// <summary>
    /// Updates the premium amount and billing frequency of the insurance policy.
    /// </summary>
    /// <param name="newPremium">The new recurring premium cost.</param>
    /// <param name="newFrequency">The new payment frequency.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newPremium"/> is null or has an amount less than or equal to zero.</exception>
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

    /// <summary>
    /// Cancels the insurance policy and records the update timestamp.
    /// </summary>
    public void Cancel()
    {
        Status = InsurancePolicyStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the insurance policy as expired and records the update timestamp.
    /// </summary>
    public void Expire()
    {
        Status = InsurancePolicyStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactivates the insurance policy back to active status and records the update timestamp.
    /// </summary>
    public void Reactivate()
    {
        Status = InsurancePolicyStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the estimated monthly equivalent cost of the insurance premium based on its payment frequency.
    /// </summary>
    /// <returns>The calculated monthly premium cost rounded to two decimal places.</returns>
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

    /// <summary>
    /// Calculates the annualized total cost of the insurance premium based on its payment frequency.
    /// </summary>
    /// <returns>The calculated annual premium cost.</returns>
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
