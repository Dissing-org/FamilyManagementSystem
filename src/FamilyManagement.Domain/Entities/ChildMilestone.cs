using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class ChildMilestone
{
    public MilestoneId Id { get; private set; }
    public ChildId ChildId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public MilestoneCategory Category { get; private set; }

    // Prospective Guideline properties (when is it expected)
    public int? ExpectedAgeMonths { get; private set; }
    public int? ExpectedWindowMaxMonths { get; private set; }
    public bool IsStandardGuideline { get; private set; }

    // Retrospective Achievement properties (when did it happen)
    public MilestoneStatus Status { get; private set; }
    public DateTime? AchievedDate { get; private set; }
    public int? AchievedAgeMonths { get; private set; }
    public string? Notes { get; private set; }
    public string? PhotoUrl { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core parameterless constructor
    private ChildMilestone()
    {
        Id = null!;
        ChildId = null!;
        Title = null!;
    }

    private ChildMilestone(
        MilestoneId id,
        ChildId childId,
        string title,
        string? description,
        MilestoneCategory category,
        int? expectedAgeMonths,
        int? expectedWindowMaxMonths,
        bool isStandardGuideline,
        MilestoneStatus status,
        DateTime? achievedDate,
        int? achievedAgeMonths,
        string? notes,
        string? photoUrl,
        DateTime createdAt)
    {
        Id = id;
        ChildId = childId;
        Title = title;
        Description = description;
        Category = category;
        ExpectedAgeMonths = expectedAgeMonths;
        ExpectedWindowMaxMonths = expectedWindowMaxMonths;
        IsStandardGuideline = isStandardGuideline;
        Status = status;
        AchievedDate = achievedDate.HasValue ? DateTime.SpecifyKind(achievedDate.Value, DateTimeKind.Utc) : null;
        AchievedAgeMonths = achievedAgeMonths;
        Notes = notes?.Trim();
        PhotoUrl = photoUrl?.Trim();
        CreatedAt = createdAt;
    }

    public static ChildMilestone CreateExpected(
        ChildId childId,
        string title,
        MilestoneCategory category,
        int expectedAgeMonths,
        int? expectedWindowMaxMonths = null,
        string? description = null,
        bool isStandardGuideline = false)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (expectedAgeMonths < 0)
        {
            throw new ArgumentException("Expected age in months cannot be negative.", nameof(expectedAgeMonths));
        }

        if (expectedWindowMaxMonths.HasValue && expectedWindowMaxMonths.Value < expectedAgeMonths)
        {
            throw new ArgumentException("Window max cannot be less than expected age.", nameof(expectedWindowMaxMonths));
        }

        return new ChildMilestone(
            MilestoneId.New(),
            childId,
            title.Trim(),
            description?.Trim(),
            category,
            expectedAgeMonths,
            expectedWindowMaxMonths,
            isStandardGuideline,
            MilestoneStatus.Expected,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);
    }

    public static ChildMilestone CreateAchieved(
        ChildId childId,
        string title,
        MilestoneCategory category,
        DateTime achievedDate,
        int achievedAgeMonths,
        string? notes = null,
        string? photoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (achievedDate > DateTime.UtcNow)
        {
            throw new ArgumentException("Achieved date cannot be in the future.", nameof(achievedDate));
        }

        return new ChildMilestone(
            MilestoneId.New(),
            childId,
            title.Trim(),
            null,
            category,
            null,
            null,
            false,
            MilestoneStatus.Achieved,
            achievedDate,
            achievedAgeMonths,
            notes,
            photoUrl,
            DateTime.UtcNow);
    }

    public void MarkAchieved(DateTime achievedDate, int achievedAgeMonths, string? notes = null, string? photoUrl = null)
    {
        if (achievedDate > DateTime.UtcNow)
        {
            throw new ArgumentException("Achieved date cannot be in the future.", nameof(achievedDate));
        }

        Status = MilestoneStatus.Achieved;
        AchievedDate = DateTime.SpecifyKind(achievedDate, DateTimeKind.Utc);
        AchievedAgeMonths = achievedAgeMonths;
        if (!string.IsNullOrWhiteSpace(notes))
        {
            Notes = notes.Trim();
        }
        if (!string.IsNullOrWhiteSpace(photoUrl))
        {
            PhotoUrl = photoUrl.Trim();
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExpectation(int expectedAgeMonths, int? expectedWindowMaxMonths, string? description)
    {
        if (expectedAgeMonths < 0)
        {
            throw new ArgumentException("Expected age in months cannot be negative.", nameof(expectedAgeMonths));
        }

        ExpectedAgeMonths = expectedAgeMonths;
        ExpectedWindowMaxMonths = expectedWindowMaxMonths;
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkSkipped(string? notes = null)
    {
        Status = MilestoneStatus.Skipped;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
