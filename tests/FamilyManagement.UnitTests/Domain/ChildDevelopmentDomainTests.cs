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
}
