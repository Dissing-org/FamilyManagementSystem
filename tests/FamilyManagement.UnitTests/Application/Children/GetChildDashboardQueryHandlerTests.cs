using FluentAssertions;
using FamilyManagement.Application.UseCases.Children.Queries;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using NSubstitute;
using Xunit;

namespace FamilyManagement.UnitTests.Application.Children;

public class GetChildDashboardQueryHandlerTests
{
    private readonly IChildProfileRepository _childRepo = Substitute.For<IChildProfileRepository>();
    private readonly IGrowthMeasurementRepository _growthRepo = Substitute.For<IGrowthMeasurementRepository>();
    private readonly IChildMilestoneRepository _milestoneRepo = Substitute.For<IChildMilestoneRepository>();

    [Fact]
    public async Task HandleAsync_ResolvesIndependentLatestMetricsCorrectly()
    {
        // Arrange
        var childId = ChildId.New();
        var child = ChildProfile.Create("Oliver", "Dissing", DateTime.UtcNow.AddYears(-2), Gender.Boy);
        _childRepo.GetByIdAsync(childId, Arg.Any<CancellationToken>()).Returns(child);

        // Measurements ordered descending by date:
        // m1: Only weight recorded today
        // m2: Only height recorded yesterday
        // m3: Both height and head circumference recorded 3 days ago
        var m1 = GrowthMeasurement.Create(childId, DateTime.UtcNow, heightCm: null, weightKg: 12.5m, headCircumferenceCm: null);
        var m2 = GrowthMeasurement.Create(childId, DateTime.UtcNow.AddDays(-1), heightCm: 88.0m, weightKg: null, headCircumferenceCm: null);
        var m3 = GrowthMeasurement.Create(childId, DateTime.UtcNow.AddDays(-3), heightCm: 87.5m, weightKg: null, headCircumferenceCm: 46.5m);

        _growthRepo.GetByChildIdAsync(child.Id, Arg.Any<CancellationToken>())
            .Returns(new List<GrowthMeasurement> { m1, m2, m3 });

        _milestoneRepo.GetByChildIdAsync(child.Id, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new List<ChildMilestone>());

        var handler = new GetChildDashboardQueryHandler(_childRepo, _growthRepo, _milestoneRepo);

        // Act
        var result = await handler.HandleAsync(new GetChildDashboardQuery(childId.Value));

        // Assert
        result.LatestMeasurement.Should().NotBeNull();
        result.LatestMeasurement!.WeightKg.Should().Be(12.5m);
        result.LatestMeasurement.HeightCm.Should().BeNull();

        result.LatestWeightMeasurement.Should().NotBeNull();
        result.LatestWeightMeasurement!.WeightKg.Should().Be(12.5m);

        result.LatestHeightMeasurement.Should().NotBeNull();
        result.LatestHeightMeasurement!.HeightCm.Should().Be(88.0m);

        result.LatestHeadCircumferenceMeasurement.Should().NotBeNull();
        result.LatestHeadCircumferenceMeasurement!.HeadCircumferenceCm.Should().Be(46.5m);
    }
}
