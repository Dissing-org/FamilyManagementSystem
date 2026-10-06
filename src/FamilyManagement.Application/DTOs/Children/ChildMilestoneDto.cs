using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Children;

public record ChildMilestoneDto(
    Guid Id,
    Guid ChildId,
    string Title,
    string? Description,
    string Category,
    string Status,
    int? ExpectedAgeMonths,
    int? ExpectedWindowMaxMonths,
    bool IsStandardGuideline,
    DateTime? AchievedDate,
    int? AchievedAgeMonths,
    string? Notes,
    string? PhotoUrl,
    DateTime CreatedAt)
{
    public static ChildMilestoneDto FromDomain(ChildMilestone m) =>
        new(
            m.Id.Value,
            m.ChildId.Value,
            m.Title,
            m.Description,
            m.Category.ToString(),
            m.Status.ToString(),
            m.ExpectedAgeMonths,
            m.ExpectedWindowMaxMonths,
            m.IsStandardGuideline,
            m.AchievedDate,
            m.AchievedAgeMonths,
            m.Notes,
            m.PhotoUrl,
            m.CreatedAt);
}
