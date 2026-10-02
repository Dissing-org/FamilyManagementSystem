using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Infrastructure.Storage;
using Xunit;

namespace FamilyManagement.UnitTests.Infrastructure;

public class GoogleDriveStorageServiceTests : IDisposable
{
    private readonly string _testDir;

    public GoogleDriveStorageServiceTests()
    {
        _testDir = Path.Combine(AppContext.BaseDirectory, "test_receipts_storage_" + Guid.NewGuid().ToString("N"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
        {
            try
            {
                Directory.Delete(_testDir, recursive: true);
            }
            catch
            {
                // Best effort cleanup in tests
            }
        }
    }

    [Theory]
    [InlineData(null, "Other")]
    [InlineData("", "Other")]
    [InlineData("   ", "Other")]
    [InlineData("Groceries", "Groceries")]
    [InlineData("Dining & Restaurant", "Dining & Restaurant")]
    [InlineData("Utilities/Bills\\Water", "UtilitiesBillsWater")]
    [InlineData("Invalid*Name?Test:<>|", "InvalidNameTest")]
    public void SanitizeFolderName_ShouldSanitizeCorrectly(string? input, string expected)
    {
        var result = GoogleDriveStorageService.SanitizeFolderName(input);
        result.Should().Be(expected);
    }

    [Fact]
    public async Task UploadAsync_WithLocalFallback_ShouldSaveInYearAndCategorySubfolder()
    {
        // Arrange
        var relativeDir = Path.GetFileName(_testDir);
        var options = Options.Create(new GoogleDriveOptions
        {
            CredentialsJsonPath = "", // triggers fallback
            UseLocalStorageFallbackWhenUnconfigured = true,
            LocalStorageFallbackDirectory = relativeDir
        });

        var service = new GoogleDriveStorageService(options, NullLogger<GoogleDriveStorageService>.Instance);
        var metadata = new ReceiptUploadMetadata(Category: "Groceries", Year: 2026, Merchant: "Albert Heijn");
        var content = new MemoryStream(Encoding.UTF8.GetBytes("fake receipt content"));

        // Act
        var result = await service.UploadAsync("receipt.pdf", content, "application/pdf", metadata);

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().NotBeNullOrWhiteSpace();
        result.FileName.Should().Contain("receipt.pdf");
        result.WebViewLink.Should().Be($"/api/receipts/files/{result.FileId}");

        var expectedFolderPath = Path.Combine(AppContext.BaseDirectory, relativeDir, "2026", "Groceries");
        Directory.Exists(expectedFolderPath).Should().BeTrue();

        var savedFiles = Directory.GetFiles(expectedFolderPath);
        savedFiles.Should().HaveCount(1);
        savedFiles[0].Should().Contain("receipt.pdf");
    }

    [Fact]
    public async Task UploadAsync_WithLocalStorageProvider_ShouldSaveDirectlyToConfiguredLocalDirectory()
    {
        // Arrange
        var options = Options.Create(new GoogleDriveOptions
        {
            StorageProvider = "Local",
            LocalStorageDirectory = _testDir
        });

        var service = new GoogleDriveStorageService(options, NullLogger<GoogleDriveStorageService>.Instance);
        var metadata = new ReceiptUploadMetadata(Category: "Dining", Year: 2026, Merchant: "Pizzeria");
        var content = new MemoryStream(Encoding.UTF8.GetBytes("pizza receipt content"));

        // Act
        var result = await service.UploadAsync("pizza.pdf", content, "application/pdf", metadata);

        // Assert
        result.Should().NotBeNull();
        result.FileId.Should().NotBeNullOrWhiteSpace();
        result.WebViewLink.Should().Be($"/api/receipts/files/{result.FileId}");

        var expectedFolderPath = Path.Combine(_testDir, "2026", "Dining");
        Directory.Exists(expectedFolderPath).Should().BeTrue();

        var savedFiles = Directory.GetFiles(expectedFolderPath);
        savedFiles.Should().HaveCount(1);
        savedFiles[0].Should().Contain("pizza.pdf");

        // Delete test
        await service.DeleteAsync(result.FileId);
        Directory.GetFiles(expectedFolderPath).Should().BeEmpty();
    }
}
