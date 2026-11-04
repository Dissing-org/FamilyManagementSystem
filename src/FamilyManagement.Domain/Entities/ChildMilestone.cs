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

    public int? ExpectedAgeMonths { get; private set; }
    public int? ExpectedWindowMaxMonths { get; private set; }
    public bool IsStandardGuideline { get; private set; }

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

    /// <summary>
    /// Creates an expected milestone for a child that has not yet been achieved.
    /// </summary>
    /// <param name="childId">The identifier of the child.</param>
    /// <param name="title">The milestone title or headline.</param>
    /// <param name="category">The developmental category of the milestone.</param>
    /// <param name="expectedAgeMonths">The expected age in months when this milestone is typically reached.</param>
    /// <param name="expectedWindowMaxMonths">The optional upper boundary age in months of the expected developmental window.</param>
    /// <param name="description">An optional description of the milestone.</param>
    /// <param name="isStandardGuideline">Indicates whether this milestone originates from standard developmental guidelines.</param>
    /// <returns>A new <see cref="ChildMilestone"/> instance initialized with expected status.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> is null or whitespace, when <paramref name="expectedAgeMonths"/> is negative, or when <paramref name="expectedWindowMaxMonths"/> is less than <paramref name="expectedAgeMonths"/>.</exception>
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

    /// <summary>
    /// Creates a milestone record that has already been achieved by the child.
    /// </summary>
    /// <param name="childId">The identifier of the child.</param>
    /// <param name="title">The milestone title or headline.</param>
    /// <param name="category">The developmental category of the milestone.</param>
    /// <param name="achievedDate">The date when the milestone was achieved.</param>
    /// <param name="achievedAgeMonths">The child's age in months when the milestone was achieved.</param>
    /// <param name="notes">Optional notes or memories about the achievement.</param>
    /// <param name="photoUrl">An optional photo URL documenting the milestone.</param>
    /// <returns>A new <see cref="ChildMilestone"/> instance initialized with achieved status.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="title"/> is null or whitespace, or when <paramref name="achievedDate"/> is in the future.</exception>
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

        if (achievedDate.Date > DateTime.UtcNow.Date.AddDays(1))
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

    /// <summary>
    /// Marks the milestone as achieved, recording the achievement date, age in months, and optional memories.
    /// </summary>
    /// <param name="achievedDate">The date when the milestone was achieved.</param>
    /// <param name="achievedAgeMonths">The child's age in months at the time of achievement.</param>
    /// <param name="notes">Optional notes or memories regarding the milestone achievement.</param>
    /// <param name="photoUrl">An optional photo URL celebrating the milestone.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="achievedDate"/> is in the future.</exception>
    public void MarkAchieved(DateTime achievedDate, int achievedAgeMonths, string? notes = null, string? photoUrl = null)
    {
        if (achievedDate.Date > DateTime.UtcNow.Date.AddDays(1))
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

    /// <summary>
    /// Updates the developmental expectation parameters for an upcoming milestone.
    /// </summary>
    /// <param name="expectedAgeMonths">The updated expected age in months.</param>
    /// <param name="expectedWindowMaxMonths">The updated optional developmental window upper boundary in months.</param>
    /// <param name="description">The updated optional description.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expectedAgeMonths"/> is negative.</exception>
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

    /// <summary>
    /// Marks the milestone as skipped with optional explanatory notes.
    /// </summary>
    /// <param name="notes">Optional notes explaining why the milestone was skipped.</param>
    public void MarkSkipped(string? notes = null)
    {
        Status = MilestoneStatus.Skipped;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
