using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.IntegrationTests;

public class ReceiptsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ReceiptsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Provide a mocked Google Drive storage service for deterministic integration tests
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IReceiptFileStorageService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var mockStorage = Substitute.For<IReceiptFileStorageService>();
                mockStorage.UploadAsync(
                    Arg.Any<string>(),
                    Arg.Any<Stream>(),
                    Arg.Any<string>(),
                    Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(GoogleDriveFileReference.Create(
                        "mock-drive-id-999",
                        "test_receipt.pdf",
                        "https://drive.google.com/file/d/mock-drive-id-999/view")));

                services.AddSingleton(mockStorage);
            });
        });
    }

    [Fact]
    public async Task GetReceipts_Initially_ShouldReturn200Ok()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/receipts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var receipts = await response.Content.ReadFromJsonAsync<List<ReceiptResponseDto>>();
        receipts.Should().NotBeNull();
    }

    [Fact]
    public async Task UploadReceipt_And_Retrieve_FullLifecycle_ShouldSucceed()
    {
        // Arrange
        var client = _factory.CreateClient();

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Whole Foods Market"), "Merchant");
        form.Add(new StringContent(DateTime.UtcNow.AddHours(-1).ToString("o")), "PurchaseDate");
        form.Add(new StringContent("84.35"), "Amount");
        form.Add(new StringContent("USD"), "Currency");
        form.Add(new StringContent("Organic groceries"), "Notes");

        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Fake PDF receipt content"));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(fileContent, "File", "receipt_wholefoods.pdf");

        // Act 1: POST /api/receipts
        var postResponse = await client.PostAsync("/api/receipts", form);

        // Assert 1: Created (201)
        var errorBody = await postResponse.Content.ReadAsStringAsync();
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created, because: errorBody);
        var created = await postResponse.Content.ReadFromJsonAsync<ReceiptResponseDto>();
        created.Should().NotBeNull();
        created!.Merchant.Should().Be("Whole Foods Market");
        created.Amount.Should().Be(84.35m);
        created.GoogleDriveFileId.Should().Be("mock-drive-id-999");
        created.WebViewLink.Should().Contain("mock-drive-id-999");

        // Act 2: GET /api/receipts/{id}
        var getResponse = await client.GetAsync($"/api/receipts/{created.Id}");

        // Assert 2: OK (200)
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ReceiptResponseDto>();
        fetched.Should().NotBeNull();
        fetched!.Id.Should().Be(created.Id);
        fetched.Merchant.Should().Be("Whole Foods Market");

        // Act 3: DELETE /api/receipts/{id}
        var deleteResponse = await client.DeleteAsync($"/api/receipts/{created.Id}");

        // Assert 3: NoContent (204)
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act 4: GET again should return 404 (archived receipts are not returned by default)
        var getAfterDelete = await client.GetAsync($"/api/receipts/{created.Id}");
        getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
