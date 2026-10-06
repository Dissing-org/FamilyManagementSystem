using FluentAssertions;
using FamilyManagement.Application.UseCases.Insurances.Queries;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace FamilyManagement.UnitTests.Application;

public class GetInsuranceOverviewQueryTests
{
    [Fact]
    public async Task HandleAsync_WithMultiCurrencyPolicies_AggregatesTotalsByCurrency()
    {
        var repository = Substitute.For<IInsurancePolicyRepository>();
        var p1 = InsurancePolicy.Create("A", "1", InsuranceCategory.Home, "H", Money.Create(100m, "DKK"), PaymentFrequency.Monthly, DateTime.UtcNow);
        var p2 = InsurancePolicy.Create("B", "2", InsuranceCategory.Auto, "C", Money.Create(50m, "USD"), PaymentFrequency.Monthly, DateTime.UtcNow);

        repository.ListAsync(
            Arg.Any<InsuranceCategory?>(),
            Arg.Any<InsurancePolicyStatus?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()).Returns(new List<InsurancePolicy> { p1, p2 });

        var handler = new GetInsuranceOverviewQueryHandler(repository);
        var result = await handler.HandleAsync(new GetInsuranceOverviewQuery());

        result.TotalsByCurrency.Should().NotBeNull();
        result.TotalsByCurrency.Should().ContainKey("DKK");
        result.TotalsByCurrency.Should().ContainKey("USD");
        result.TotalsByCurrency!["DKK"].TotalMonthlyCost.Should().Be(100m);
        result.TotalsByCurrency!["USD"].TotalMonthlyCost.Should().Be(50m);
    }

    [Fact]
    public async Task HandleAsync_IncludesPolicyRenewingToday()
    {
        var repository = Substitute.For<IInsurancePolicyRepository>();
        var today = DateTime.UtcNow;
        var policy = InsurancePolicy.Create("A", "1", InsuranceCategory.Home, "H", Money.Create(100m, "DKK"), PaymentFrequency.Monthly, today.AddYears(-1), today);

        repository.ListAsync(
            Arg.Any<InsuranceCategory?>(),
            Arg.Any<InsurancePolicyStatus?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>()).Returns(new List<InsurancePolicy> { policy });

        var handler = new GetInsuranceOverviewQueryHandler(repository);
        var result = await handler.HandleAsync(new GetInsuranceOverviewQuery(DaysAhead: 30));

        result.UpcomingRenewals.Should().HaveCount(1);
        result.UpcomingRenewals[0].DaysUntilRenewal.Should().Be(0);
    }
}
