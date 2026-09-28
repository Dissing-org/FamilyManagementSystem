using FluentAssertions;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class ReceiptTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnActiveReceipt()
    {
        // Arrange
        var merchant = "Supermarket ABC";
        var purchaseDate = new DateTime(2026, 3, 15, 14, 30, 0, DateTimeKind.Utc);
        var money = Money.Create(49.99m, "USD");
        var fileRef = GoogleDriveFileReference.Create("drive-file-id-123", "receipt_123.pdf", "https://drive.google.com/file/d/123/view");

        // Act
        var receipt = Receipt.Create(merchant, purchaseDate, money, fileRef, "Groceries", "Grocery shopping");

        // Assert
        receipt.Id.Value.Should().NotBeEmpty();
        receipt.Merchant.Should().Be(merchant);
        receipt.PurchaseDate.Should().Be(purchaseDate);
        receipt.Amount.Should().Be(money);
        receipt.FileReference.Should().Be(fileRef);
        receipt.Category.Should().Be("Groceries");
        receipt.Notes.Should().Be("Grocery shopping");
        receipt.Status.Should().Be(ReceiptStatus.Active);
        receipt.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidMerchant_ShouldThrowArgumentException(string? invalidMerchant)
    {
        // Arrange
        var purchaseDate = DateTime.UtcNow.AddDays(-1);
        var money = Money.Create(10m, "USD");
        var fileRef = GoogleDriveFileReference.Create("drive-id", "receipt.pdf", "https://drive.google.com/file/d/123/view");

        // Act
        var act = () => Receipt.Create(invalidMerchant!, purchaseDate, money, fileRef);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Merchant is required*");
    }

    [Fact]
    public void Create_WithFuturePurchaseDate_ShouldThrowArgumentException()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(5);
        var money = Money.Create(10m, "USD");
        var fileRef = GoogleDriveFileReference.Create("drive-id", "receipt.pdf", "https://drive.google.com/file/d/123/view");

        // Act
        var act = () => Receipt.Create("Merchant", futureDate, money, fileRef);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Purchase date cannot be in the future*");
    }

    [Fact]
    public void Archive_ActiveReceipt_ShouldSetStatusToArchived()
    {
        // Arrange
        var receipt = Receipt.Create(
            "Merchant",
            DateTime.UtcNow.AddDays(-1),
            Money.Create(25m, "USD"),
            GoogleDriveFileReference.Create("id", "name.jpg", "https://drive.google.com/view"));

        // Act
        receipt.Archive();

        // Assert
        receipt.Status.Should().Be(ReceiptStatus.Archived);
        receipt.ArchivedAt.Should().NotBeNull();
    }
}
