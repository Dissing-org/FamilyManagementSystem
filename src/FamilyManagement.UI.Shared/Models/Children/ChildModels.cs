namespace FamilyManagement.UI.Shared.Models.Children;

public class ChildWardrobeSizesModel
{
    public string? ClothesSize { get; set; }
    public string? ShoeSize { get; set; }
    public string? HatSize { get; set; }
    public string? DiaperSize { get; set; }
}

public class ChildAgeModel
{
    public int Years { get; set; }
    public int Months { get; set; }
    public int Days { get; set; }
    public int TotalMonths { get; set; }
    public int TotalWeeks { get; set; }
    public int TotalDays { get; set; }
    public string Formatted { get; set; } = string.Empty;
}

public class ChildProfileModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "Boy";
    public string? AvatarUrl { get; set; }
    public ChildWardrobeSizesModel Sizes { get; set; } = new();
    public ChildAgeModel Age { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string FullName => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";
}

public class GrowthMeasurementModel
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public DateTime RecordedDate { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeadCircumferenceCm { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ChildMilestoneModel
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "GrossMotor";
    public string Status { get; set; } = "Expected";
    public int? ExpectedAgeMonths { get; set; }
    public int? ExpectedWindowMaxMonths { get; set; }
    public bool IsStandardGuideline { get; set; }
    public DateTime? AchievedDate { get; set; }
    public int? AchievedAgeMonths { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsAchieved => Status == "Achieved";
}

public class ChildDashboardModel
{
    public ChildProfileModel Child { get; set; } = new();
    public GrowthMeasurementModel? LatestMeasurement { get; set; }
    public GrowthMeasurementModel? LatestHeightMeasurement { get; set; }
    public GrowthMeasurementModel? LatestWeightMeasurement { get; set; }
    public GrowthMeasurementModel? LatestHeadCircumferenceMeasurement { get; set; }
    public int TotalMeasurements { get; set; }
    public int AchievedMilestonesCount { get; set; }
    public int PendingMilestonesCount { get; set; }
    public List<ChildMilestoneModel> RecentAchievements { get; set; } = new();
    public List<ChildMilestoneModel> UpcomingExpectations { get; set; } = new();
}
