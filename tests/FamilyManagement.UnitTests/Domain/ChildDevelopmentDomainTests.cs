using FluentAssertions;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class ChildDevelopmentDomainTests
{
    [Fact]
    public void ChildAge_CalculatesCorrectYearsAndMonths()
    {
        var dob = new DateTime(2025, 4, 15);
        var asOf = new DateTime(2026, 10, 15);

        var age = ChildAge.Calculate(dob, asOf);

        age.Years.Should().Be(1);
        age.Months.Should().Be(6);
        age.TotalMonths.Should().Be(18);
        age.Formatted.Should().Be("1 yr 6 mos");
    }

    [Fact]
    public void ChildAge_UnderOneYear_CalculatesMonthsAndDays()
    {
        var dob = new DateTime(2026, 4, 1);
        var asOf = new DateTime(2026, 10, 6);

        var age = ChildAge.Calculate(dob, asOf);

        age.Years.Should().Be(0);
        age.TotalMonths.Should().Be(6);
        age.Days.Should().Be(5);
        age.Formatted.Should().Be("6 mos 5 d");
    }

    [Fact]
    public void ChildProfile_Create_ValidInputs_InitializesProfile()
    {
        var dob = DateTime.UtcNow.AddMonths(-14);
        var sizes = new ChildWardrobeSizes("86", "22 EU", "48 cm", "Size 4");

        var child = ChildProfile.Create("Oliver", "Dissing", dob, Gender.Boy, initialSizes: sizes);

        child.FirstName.Should().Be("Oliver");
        child.LastName.Should().Be("Dissing");
        child.Gender.Should().Be(Gender.Boy);
        child.CurrentSizes.ClothesSize.Should().Be("86");
        child.CurrentSizes.ShoeSize.Should().Be("22 EU");
        child.GetAge().Years.Should().Be(1);
    }

    [Fact]
    public void ChildMilestone_MarkAchieved_SetsStatusAndAchievedDate()
    {
        var childId = ChildId.New();
        var milestone = ChildMilestone.CreateExpected(
            childId,
            "Walking independently",
            MilestoneCategory.GrossMotor,
            expectedAgeMonths: 12,
            expectedWindowMaxMonths: 15);

        milestone.Status.Should().Be(MilestoneStatus.Expected);

        var achievedDate = DateTime.UtcNow.AddDays(-10);
        milestone.MarkAchieved(achievedDate, achievedAgeMonths: 13, "Took 10 steps across living room!");

        milestone.Status.Should().Be(MilestoneStatus.Achieved);
        milestone.AchievedDate.Should().NotBeNull();
        milestone.AchievedAgeMonths.Should().Be(13);
        milestone.Notes.Should().Be("Took 10 steps across living room!");
    }

    [Fact]
    public void GrowthMeasurement_Create_EnforcesAtLeastOneDimension()
    {
        var childId = ChildId.New();
        var act = () => GrowthMeasurement.Create(childId, DateTime.UtcNow, null, null, null);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*At least one measurement*");
    }

    [Theory]
    [InlineData("2023-01-31", "2023-03-01", 0, 1, 29, 1)] // Jan 31 to March 1 (non-leap year Feb 28 days) -> 1 mo 1 day or 29 days depending on calendar month advancement
    [InlineData("2024-01-31", "2024-03-01", 0, 1, 30, 1)] // Leap year (Feb 29)
    [InlineData("2025-05-31", "2025-07-01", 0, 1, 31, 1)] // May 31 to July 1
    public void ChildAge_MonthEndEdgeCases_NeverYieldsNegativeDays(
        string dobStr,
        string asOfStr,
        int expectedYears,
        int expectedMonths,
        int expectedTotalDaysMin,
        int expectedDaysMin)
    {
        var dob = DateTime.Parse(dobStr);
        var asOf = DateTime.Parse(asOfStr);

        var age = ChildAge.Calculate(dob, asOf);

        age.Years.Should().Be(expectedYears);
        age.Months.Should().Be(expectedMonths);
        age.Days.Should().BeGreaterThanOrEqualTo(expectedDaysMin);
        age.TotalDays.Should().BeGreaterThanOrEqualTo(expectedTotalDaysMin);
    }
}
