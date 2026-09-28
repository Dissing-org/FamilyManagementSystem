using System.Text;
using FluentAssertions;
using NSubstitute;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Application.UseCases.Receipts.Commands;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Application;

public class UploadReceiptCommandHandlerTests
{
    private readonly IReceiptRepository _repository = Substitute.For<IReceiptRepository>();
    private readonly IReceiptFileStorageService _storageService = Substitute.For<IReceiptFileStorageService>();

    [Fact]
    public async Task Handle_ValidCommand_ShouldUploadToDriveAndSaveToRepository()
    {
        // Arrange
        var command = new UploadReceiptCommand(
            Merchant: "Costco Wholesale",
            PurchaseDate: new DateTime(2026, 3, 20, 10, 0, 0, DateTimeKind.Utc),
            Amount: 125.50m,
            Currency: "USD",
            FileName: "costco_receipt.pdf",
            FileContent: new MemoryStream(Encoding.UTF8.GetBytes("fake-pdf-content")),
            ContentType: "application/pdf",
            Notes: "Weekly groceries");

        var fileRef = GoogleDriveFileReference.Create(
            "drive-file-id-456",
            "costco_receipt.pdf",
            "https://drive.google.com/file/d/456/view");

        _storageService.UploadAsync(
            command.FileName,
            Arg.Any<Stream>(),
            command.ContentType,
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(fileRef));

        var handler = new UploadReceiptCommandHandler(_repository, _storageService);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Merchant.Should().Be("Costco Wholesale");
        result.Amount.Should().Be(125.50m);
        result.Currency.Should().Be("USD");
        result.GoogleDriveFileId.Should().Be("drive-file-id-456");
        result.WebViewLink.Should().Be("https://drive.google.com/file/d/456/view");

        await _storageService.Received(1).UploadAsync(
            command.FileName,
            Arg.Any<Stream>(),
            command.ContentType,
            Arg.Any<CancellationToken>());

        await _repository.Received(1).AddAsync(
            Arg.Is<Receipt>(r => r.Merchant == "Costco Wholesale" && r.Amount.Amount == 125.50m),
            Arg.Any<CancellationToken>());

        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
