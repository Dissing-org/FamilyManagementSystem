namespace FamilyManagement.Application.DTOs.Children;

public record ChildDashboardDto(
    ChildProfileDto Child,
    GrowthMeasurementDto? LatestMeasurement,
    int TotalMeasurements,
    int AchievedMilestonesCount,
    int PendingMilestonesCount,
    List<ChildMilestoneDto> RecentAchievements,
    List<ChildMilestoneDto> UpcomingExpectations);
