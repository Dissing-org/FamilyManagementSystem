using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Application.DTOs.Children;

public record ChildWardrobeSizesDto(
    string? ClothesSize,
    string? ShoeSize,
    string? HatSize,
    string? DiaperSize);

public record ChildAgeDto(
    int Years,
    int Months,
    int Days,
    int TotalMonths,
    int TotalWeeks,
    int TotalDays,
    string Formatted);

public record ChildProfileDto(
    Guid Id,
    string FirstName,
    string? LastName,
    DateTime DateOfBirth,
    string Gender,
    string? AvatarUrl,
    ChildWardrobeSizesDto Sizes,
    ChildAgeDto Age,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static ChildProfileDto FromDomain(ChildProfile child, DateTime? asOfDate = null)
    {
        var age = child.GetAge(asOfDate);
        return new ChildProfileDto(
            child.Id.Value,
            child.FirstName,
            child.LastName,
            child.DateOfBirth,
            child.Gender.ToString(),
            child.AvatarUrl,
            new ChildWardrobeSizesDto(
                child.CurrentSizes.ClothesSize,
                child.CurrentSizes.ShoeSize,
                child.CurrentSizes.HatSize,
                child.CurrentSizes.DiaperSize),
            new ChildAgeDto(
                age.Years,
                age.Months,
                age.Days,
                age.TotalMonths,
                age.TotalWeeks,
                age.TotalDays,
                age.Formatted),
            child.CreatedAt,
            child.UpdatedAt);
    }
}
