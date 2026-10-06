using FluentAssertions;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class InsurancePolicyTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnActivePolicy()
    {
        // Arrange
        var insurer = "Topdanmark";
        var policyNumber = "POL-12345";
        var category = InsuranceCategory.Home;
        var insuredParty = "Main Family Residence";
        var premium = Money.Create(1200m, "DKK");
        var frequency = PaymentFrequency.Annually;
        var startDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var renewalDate = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var deductible = Money.Create(2500m, "DKK");
        var notes = "Includes water damage and fire coverage";

        // Act
        var policy = InsurancePolicy.Create(
            insurer,
            policyNumber,
            category,
            insuredParty,
            premium,
            frequency,
            startDate,
            renewalDate,
            deductible,
            notes);

        // Assert
        policy.Id.Value.Should().NotBeEmpty();
        policy.Insurer.Should().Be("Topdanmark");
        policy.PolicyNumber.Should().Be("POL-12345");
        policy.Category.Should().Be(InsuranceCategory.Home);
        policy.InsuredParty.Should().Be("Main Family Residence");
        policy.Premium.Should().Be(premium);
        policy.Frequency.Should().Be(PaymentFrequency.Annually);
        policy.StartDate.Should().Be(startDate);
        policy.RenewalDate.Should().Be(renewalDate);
        policy.Deductible.Should().Be(deductible);
        policy.Notes.Should().Be(notes);
        policy.Status.Should().Be(InsurancePolicyStatus.Active);
        policy.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidInsurer_ShouldThrowArgumentException(string? invalidInsurer)
    {
        var act = () => InsurancePolicy.Create(
            invalidInsurer!,
            "POL-1",
            InsuranceCategory.Health,
            "John Doe",
            Money.Create(100m, "USD"),
            PaymentFrequency.Monthly,
            DateTime.UtcNow);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Insurer is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidInsuredParty_ShouldThrowArgumentException(string? invalidParty)
    {
        var act = () => InsurancePolicy.Create(
            "Allianz",
            "POL-1",
            InsuranceCategory.Auto,
            invalidParty!,
            Money.Create(100m, "USD"),
            PaymentFrequency.Monthly,
            DateTime.UtcNow);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Insured party or asset is required*");
    }

    [Theory]
    [InlineData(PaymentFrequency.Monthly, 100, 100, 1200)]
    [InlineData(PaymentFrequency.Quarterly, 300, 100, 1200)]
    [InlineData(PaymentFrequency.SemiAnnually, 600, 100, 1200)]
    [InlineData(PaymentFrequency.Annually, 1200, 100, 1200)]
    public void CostCalculations_ShouldNormalizeMonthlyAndAnnualCorrectly(
        PaymentFrequency frequency,
        decimal amount,
        decimal expectedMonthly,
        decimal expectedAnnual)
    {
        var policy = InsurancePolicy.Create(
            "Tryg",
            "POL-TEST",
            InsuranceCategory.Auto,
            "Family Car",
            Money.Create(amount, "DKK"),
            frequency,
            DateTime.UtcNow);

        policy.CalculateMonthlyCost().Should().Be(expectedMonthly);
        policy.CalculateAnnualCost().Should().Be(expectedAnnual);
    }

    [Fact]
    public void Cancel_ActivePolicy_ShouldUpdateStatusAndUpdatedAt()
    {
        var policy = InsurancePolicy.Create(
            "Tryg",
            "POL-TEST",
            InsuranceCategory.Travel,
            "All Family Members",
            Money.Create(50m, "USD"),
            PaymentFrequency.Monthly,
            DateTime.UtcNow.AddMonths(-2));

        policy.Cancel();

        policy.Status.Should().Be(InsurancePolicyStatus.Cancelled);
        policy.UpdatedAt.Should().NotBeNull();
        policy.UpdatedAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateDetails_ShouldModifyEditableFields()
    {
        var originalStart = DateTime.UtcNow.AddMonths(-6);
        var policy = InsurancePolicy.Create(
            "Original Insurer",
            "POL-ORIG",
            InsuranceCategory.Home,
            "Apartment 1",
            Money.Create(200m, "EUR"),
            PaymentFrequency.Monthly,
            originalStart);

        var newStart = DateTime.UtcNow.AddMonths(-1);
        var newRenewal = DateTime.UtcNow.AddMonths(6);
        policy.UpdateDetails(
            "Updated Insurer",
            "POL-NEW",
            InsuranceCategory.Liability,
            "Apartment 2",
            newStart,
            newRenewal,
            Money.Create(500m, "EUR"),
            "New updated policy terms");

        policy.Insurer.Should().Be("Updated Insurer");
        policy.PolicyNumber.Should().Be("POL-NEW");
        policy.Category.Should().Be(InsuranceCategory.Liability);
        policy.InsuredParty.Should().Be("Apartment 2");
        policy.StartDate.Should().Be(newStart);
        policy.RenewalDate.Should().Be(newRenewal);
        policy.Deductible!.Amount.Should().Be(500m);
        policy.Notes.Should().Be("New updated policy terms");
        policy.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateDetails_WhenRenewalDateBeforeStartDate_ShouldThrowArgumentException()
    {
        var policy = InsurancePolicy.Create(
            "Insurer",
            "POL-1",
            InsuranceCategory.Auto,
            "Car",
            Money.Create(100m, "USD"),
            PaymentFrequency.Monthly,
            DateTime.UtcNow);

        var act = () => policy.UpdateDetails(
            "Insurer",
            "POL-1",
            InsuranceCategory.Auto,
            "Car",
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(-1),
            null,
            null);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Renewal date cannot be earlier than start date*");
    }
}
