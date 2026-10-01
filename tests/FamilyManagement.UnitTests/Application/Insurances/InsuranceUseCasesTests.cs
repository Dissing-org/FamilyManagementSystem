using FluentAssertions;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Commands;
using FamilyManagement.Application.UseCases.Insurances.Queries;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace FamilyManagement.UnitTests.Application.Insurances;

public class InsuranceUseCasesTests
{
    private readonly IInsurancePolicyRepository _repository = Substitute.For<IInsurancePolicyRepository>();

    [Fact]
    public async Task CreateInsurancePolicy_ValidCommand_PersistsAndReturnsDto()
    {
        // Arrange
        var handler = new CreateInsurancePolicyCommandHandler(_repository);
        var command = new CreateInsurancePolicyCommand(
            "Tryg",
            "POL-99",
            InsuranceCategory.Home,
            "Summer House",
            300m,
            "DKK",
            PaymentFrequency.Quarterly,
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            1000m,
            "Covers water & fire damage");

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Insurer.Should().Be("Tryg");
        result.PolicyNumber.Should().Be("POL-99");
        result.Category.Should().Be("Home");
        result.InsuredParty.Should().Be("Summer House");
        result.PremiumAmount.Should().Be(300m);
        result.Frequency.Should().Be("Quarterly");
        result.MonthlyCost.Should().Be(100m);
        result.AnnualCost.Should().Be(1200m);

        await _repository.Received(1).AddAsync(Arg.Is<InsurancePolicy>(p =>
            p.Insurer == "Tryg" &&
            p.PolicyNumber == "POL-99" &&
            p.Category == InsuranceCategory.Home &&
            p.Premium.Amount == 300m));
    }

    [Fact]
    public async Task GetInsuranceOverview_CalculatesTotalsAndUpcomingRenewals()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var policy1 = InsurancePolicy.Create(
            "Tryg", "POL-1", InsuranceCategory.Home, "Apartment",
            Money.Create(1200m, "DKK"), PaymentFrequency.Annually, now.AddMonths(-6),
            renewalDate: now.AddDays(20)); // Expiring in 20 days

        var policy2 = InsurancePolicy.Create(
            "Topdanmark", "POL-2", InsuranceCategory.Auto, "Car",
            Money.Create(500m, "DKK"), PaymentFrequency.Monthly, now.AddMonths(-1),
            renewalDate: now.AddDays(150)); // Expiring in 150 days (beyond 90 days)

        var cancelledPolicy = InsurancePolicy.Create(
            "Old Insurer", "POL-3", InsuranceCategory.Travel, "Family",
            Money.Create(200m, "DKK"), PaymentFrequency.Annually, now.AddYears(-1));
        cancelledPolicy.Cancel();

        _repository.ListAsync(null, null, null).Returns(new List<InsurancePolicy> { policy1, policy2, cancelledPolicy });

        var handler = new GetInsuranceOverviewQueryHandler(_repository);

        // Act
        var overview = await handler.HandleAsync(new GetInsuranceOverviewQuery(DaysAhead: 90));

        // Assert
        overview.TotalPolicies.Should().Be(3);
        overview.ActivePolicies.Should().Be(2);

        // policy1 monthly = 100, annual = 1200. policy2 monthly = 500, annual = 6000
        overview.TotalMonthlyCost.Should().Be(600m);
        overview.TotalAnnualCost.Should().Be(7200m);

        // Category breakdown
        overview.CostByCategory.Should().ContainKey("Home");
        overview.CostByCategory["Home"].Should().Be(1200m);
        overview.CostByCategory.Should().ContainKey("Auto");
        overview.CostByCategory["Auto"].Should().Be(6000m);

        // Upcoming renewals within 90 days: only policy1 (in 20 days)
        overview.UpcomingRenewals.Should().HaveCount(1);
        overview.UpcomingRenewals[0].Insurer.Should().Be("Tryg");
        overview.UpcomingRenewals[0].DaysUntilRenewal.Should().BeInRange(19, 21);
    }
}
