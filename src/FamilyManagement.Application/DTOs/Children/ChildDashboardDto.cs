namespace FamilyManagement.Application.DTOs.Children;

public record ChildDashboardDto(
    ChildProfileDto Child,
    GrowthMeasurementDto? LatestMeasurement,
    GrowthMeasurementDto? LatestHeightMeasurement,
    GrowthMeasurementDto? LatestWeightMeasurement,
    GrowthMeasurementDto? LatestHeadCircumferenceMeasurement,
    int TotalMeasurements,
    int AchievedMilestonesCount,
    int PendingMilestonesCount,
    List<ChildMilestoneDto> RecentAchievements,
    List<ChildMilestoneDto> UpcomingExpectations);
