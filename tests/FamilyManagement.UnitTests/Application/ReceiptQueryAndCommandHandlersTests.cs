using FluentAssertions;
using NSubstitute;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Application.UseCases.Receipts.Commands;
using FamilyManagement.Application.UseCases.Receipts.Queries;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Application;

public class ReceiptQueryAndCommandHandlersTests
{
    private readonly IReceiptRepository _repository = Substitute.For<IReceiptRepository>();
    private readonly IReceiptFileStorageService _storageService = Substitute.For<IReceiptFileStorageService>();

    [Fact]
    public async Task GetReceiptById_WhenExists_ShouldReturnDto()
    {
        // Arrange
        var receipt = Receipt.Create(
            "Target",
            DateTime.UtcNow.AddDays(-2),
            Money.Create(35.20m, "USD"),
            GoogleDriveFileReference.Create("drive-id-1", "target.jpg", "https://drive.google.com/view/1"));

        _repository.GetByIdAsync(receipt.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Receipt?>(receipt));

        var handler = new GetReceiptByIdQueryHandler(_repository);

        // Act
        var result = await handler.HandleAsync(new GetReceiptByIdQuery(receipt.Id.Value));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(receipt.Id.Value);
        result.Merchant.Should().Be("Target");
        result.Amount.Should().Be(35.20m);
    }

    [Fact]
    public async Task GetReceiptById_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(Arg.Any<ReceiptId>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Receipt?>(null));

        var handler = new GetReceiptByIdQueryHandler(_repository);

        // Act
        var result = await handler.HandleAsync(new GetReceiptByIdQuery(id));

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ListReceipts_ShouldReturnMappedDtos()
    {
        // Arrange
        var receipt1 = Receipt.Create("Store A", DateTime.UtcNow.AddDays(-1), Money.Create(10m, "USD"), GoogleDriveFileReference.Create("id-1", "a.pdf"));
        var receipt2 = Receipt.Create("Store B", DateTime.UtcNow.AddDays(-2), Money.Create(20m, "USD"), GoogleDriveFileReference.Create("id-2", "b.pdf"));

        _repository.ListAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<Receipt>>(new List<Receipt> { receipt1, receipt2 }));

        var handler = new ListReceiptsQueryHandler(_repository);

        // Act
        var result = await handler.HandleAsync(new ListReceiptsQuery());

        // Assert
        result.Should().HaveCount(2);
        result.Select(r => r.Merchant).Should().Contain(new[] { "Store A", "Store B" });
    }

    [Fact]
    public async Task DeleteReceipt_WhenExists_ShouldArchiveAndSave()
    {
        // Arrange
        var receipt = Receipt.Create("Store C", DateTime.UtcNow.AddDays(-1), Money.Create(15m, "USD"), GoogleDriveFileReference.Create("id-3", "c.pdf"));
        _repository.GetByIdAsync(receipt.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Receipt?>(receipt));

        var handler = new DeleteReceiptCommandHandler(_repository);

        // Act
        var result = await handler.HandleAsync(new DeleteReceiptCommand(receipt.Id.Value));

        // Assert
        result.Should().BeTrue();
        receipt.Status.Should().Be(ReceiptStatus.Archived);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
