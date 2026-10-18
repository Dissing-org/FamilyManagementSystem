using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Services;
using FamilyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class FreezerDomainTests
{
    [Fact]
    public void FreezerId_WhenEmptyGuidProvided_ThrowsArgumentException()
    {
        var action = () => new FreezerId(Guid.Empty);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerCompartmentId_WhenEmptyGuidProvided_ThrowsArgumentException()
    {
        var action = () => new FreezerCompartmentId(Guid.Empty);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerItemId_WhenEmptyGuidProvided_ThrowsArgumentException()
    {
        var action = () => new FreezerItemId(Guid.Empty);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerTransactionId_WhenEmptyGuidProvided_ThrowsArgumentException()
    {
        var action = () => new FreezerTransactionId(Guid.Empty);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Freezer_Create_WithValidArgs_InitializesCorrectly()
    {
        var freezer = Freezer.Create("Kitchen Freezer", "Utility Room", 1);

        freezer.Id.Value.Should().NotBeEmpty();
        freezer.Name.Should().Be("Kitchen Freezer");
        freezer.Location.Should().Be("Utility Room");
        freezer.DisplayOrder.Should().Be(1);
        freezer.Compartments.Should().BeEmpty();
    }

    [Fact]
    public void Freezer_Create_WithEmptyName_ThrowsArgumentException()
    {
        var action = () => Freezer.Create("  ");
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Freezer_AddCompartment_AddsCompartmentSuccessfully()
    {
        var freezer = Freezer.Create("Garage Chest Freezer");
        var compartment = freezer.AddCompartment("Top Drawer", 0);

        freezer.Compartments.Should().ContainSingle();
        compartment.FreezerId.Should().Be(freezer.Id);
        compartment.Name.Should().Be("Top Drawer");
    }

    [Fact]
    public void Freezer_RemoveCompartment_RemovesCorrectly()
    {
        var freezer = Freezer.Create("Main Freezer");
        var comp = freezer.AddCompartment("Drawer 1");

        freezer.Compartments.Should().ContainSingle();
        freezer.RemoveCompartment(comp.Id);
        freezer.Compartments.Should().BeEmpty();
    }

    [Fact]
    public void Freezer_UpdateDetails_UpdatesFields()
    {
        var freezer = Freezer.Create("Old Name", "Kitchen", 0);
        freezer.UpdateDetails("New Name", "Basement", 2);

        freezer.Name.Should().Be("New Name");
        freezer.Location.Should().Be("Basement");
        freezer.DisplayOrder.Should().Be(2);
    }

    [Fact]
    public void FreezerItem_Create_DefaultExpiryUsesCategoryShelfLife()
    {
        var freezeDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var item = FreezerItem.Create(
            "Minced Beef",
            FreezerCompartmentId.New(),
            FreezerCategory.MeatAndPoultry,
            2,
            FreezerUnit.Packages,
            freezeDate);

        var expectedExpiry = freezeDate.Add(FreezerShelfLifeRules.GetRecommendedShelfLife(FreezerCategory.MeatAndPoultry));
        item.ExpiryDate.Should().Be(expectedExpiry);
        item.Quantity.Should().Be(2);
    }

    [Fact]
    public void FreezerItem_Create_NegativeQuantity_ThrowsArgumentException()
    {
        var action = () => FreezerItem.Create(
            "Fish",
            FreezerCompartmentId.New(),
            FreezerCategory.Seafood,
            -1,
            FreezerUnit.Packages,
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerItem_Consume_ReducesQuantityAndCreatesTransaction()
    {
        var item = FreezerItem.Create(
            "Chicken Breasts",
            FreezerCompartmentId.New(),
            FreezerCategory.MeatAndPoultry,
            5,
            FreezerUnit.Pieces,
            DateTime.UtcNow);

        var tx = item.Consume(2, ConsumptionReason.Consumed, "Dinner");

        item.Quantity.Should().Be(3);
        tx.ItemId.Should().Be(item.Id);
        tx.QuantityChanged.Should().Be(2);
        tx.RemainingQuantity.Should().Be(3);
        tx.Reason.Should().Be(ConsumptionReason.Consumed);
        tx.Notes.Should().Be("Dinner");
    }

    [Fact]
    public void FreezerItem_Consume_MoreThanAvailable_ThrowsInvalidOperationException()
    {
        var item = FreezerItem.Create(
            "Ice Cream",
            FreezerCompartmentId.New(),
            FreezerCategory.DessertsAndIceCream,
            1,
            FreezerUnit.Liters,
            DateTime.UtcNow);

        var action = () => item.Consume(2, ConsumptionReason.Consumed);
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void FreezerItem_AddQuantity_IncreasesQuantity()
    {
        var item = FreezerItem.Create(
            "Peas",
            FreezerCompartmentId.New(),
            FreezerCategory.VegetablesAndFruits,
            1,
            FreezerUnit.Kilograms,
            DateTime.UtcNow);

        item.AddQuantity(1.5m);
        item.Quantity.Should().Be(2.5m);
    }

    [Fact]
    public void FreezerItem_MoveToCompartment_UpdatesCompartmentId()
    {
        var comp1 = FreezerCompartmentId.New();
        var comp2 = FreezerCompartmentId.New();
        var item = FreezerItem.Create(
            "Bread",
            comp1,
            FreezerCategory.Bakery,
            1,
            FreezerUnit.Pieces,
            DateTime.UtcNow);

        item.MoveToCompartment(comp2);
        item.CompartmentId.Should().Be(comp2);
    }

    [Theory]
    [InlineData(30, FreshnessStatus.Fresh)]
    [InlineData(7, FreshnessStatus.ExpiringSoon)]
    [InlineData(-1, FreshnessStatus.Expired)]
    public void FreezerItem_EvaluateFreshness_EvaluatesAccurately(int daysUntilExpiry, FreshnessStatus expected)
    {
        var reference = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var expiry = reference.AddDays(daysUntilExpiry);
        var item = FreezerItem.Create(
            "Test item",
            FreezerCompartmentId.New(),
            FreezerCategory.Other,
            1,
            FreezerUnit.Pieces,
            reference.AddDays(-30),
            expiryDate: expiry);

        item.EvaluateFreshness(reference).Should().Be(expected);
    }

    [Fact]
    public void FreezerItem_IsLowStock_ReturnsTrueWhenQuantityAtOrBelowMinimum()
    {
        var item = FreezerItem.Create(
            "Butter",
            FreezerCompartmentId.New(),
            FreezerCategory.Bakery,
            1,
            FreezerUnit.Pieces,
            DateTime.UtcNow,
            minimumQuantity: 2);

        item.IsLowStock().Should().BeTrue();

        item.AddQuantity(2);
        item.IsLowStock().Should().BeFalse();
    }

    [Fact]
    public void FreezerShelfLifeRules_ProvidesRecommendedTimes()
    {
        FreezerShelfLifeRules.GetRecommendedShelfLife(FreezerCategory.PreparedMeals)
            .Should().Be(TimeSpan.FromDays(90));
        FreezerShelfLifeRules.GetRecommendedShelfLife(FreezerCategory.VegetablesAndFruits)
            .Should().Be(TimeSpan.FromDays(300));
    }
}
