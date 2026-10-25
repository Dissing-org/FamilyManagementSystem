using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class FreezerCatalogDomainTests
{
    [Fact]
    public void FreezerCatalogItemId_WhenEmptyGuidProvided_ThrowsArgumentException()
    {
        var action = () => new FreezerCatalogItemId(Guid.Empty);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerCatalogItem_Create_WithValidArgs_InitializesCorrectly()
    {
        var item = FreezerCatalogItem.Create(
            "  Hakket oksekød  ",
            FreezerCategory.MeatAndPoultry,
            FreezerUnit.Packages,
            2);

        item.Id.Value.Should().NotBeEmpty();
        item.Name.Should().Be("Hakket oksekød");
        item.DefaultCategory.Should().Be(FreezerCategory.MeatAndPoultry);
        item.DefaultUnit.Should().Be(FreezerUnit.Packages);
        item.DefaultMinimumQuantity.Should().Be(2);
        item.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        item.LastUsedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FreezerCatalogItem_Create_WithEmptyName_ThrowsArgumentException(string? name)
    {
        var action = () => FreezerCatalogItem.Create(name!, FreezerCategory.Other, FreezerUnit.Pieces);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerCatalogItem_Create_WithNegativeMinQty_ThrowsArgumentException()
    {
        var action = () => FreezerCatalogItem.Create("Suppe", FreezerCategory.PreparedMeals, FreezerUnit.Portions, -1);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FreezerCatalogItem_UpdateDefaults_UpdatesCategoryUnitAndTouchUsage()
    {
        var item = FreezerCatalogItem.Create("Laks", FreezerCategory.Seafood, FreezerUnit.Pieces, 1);
        var originalUsedAt = item.LastUsedAt;

        item.UpdateDefaults(FreezerCategory.Seafood, FreezerUnit.Packages, 3);

        item.DefaultUnit.Should().Be(FreezerUnit.Packages);
        item.DefaultMinimumQuantity.Should().Be(3);
        item.LastUsedAt.Should().BeOnOrAfter(originalUsedAt);
    }

    [Fact]
    public void FreezerCatalogItem_TouchUsage_UpdatesLastUsedAt()
    {
        var item = FreezerCatalogItem.Create("Is", FreezerCategory.DessertsAndIceCream, FreezerUnit.Liters);
        var old = item.LastUsedAt;

        item.TouchUsage();

        item.LastUsedAt.Should().BeOnOrAfter(old);
    }
}
