using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.IntegrationTests;

/// <summary>
/// Verifies that a database created by an older version of the app (with the Amount and Currency
/// columns that receipts no longer track) is migrated on startup and still accepts new receipts.
/// </summary>
public class ReceiptsLegacySchemaMigrationTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"legacy_receipts_{Guid.NewGuid():N}.db");

    [Fact]
    public async Task Startup_WithLegacyAmountAndCurrencyColumns_ShouldDropThemAndAcceptNewReceipts()
    {
        // Arrange: a legacy Receipts table where Amount and Currency are NOT NULL
        using (var connection = new SqliteConnection($"Data Source={_dbPath};Pooling=False"))
        {
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE "Receipts" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_Receipts" PRIMARY KEY,
                    "Merchant" TEXT NOT NULL,
                    "PurchaseDate" TEXT NOT NULL,
                    "Amount" TEXT NOT NULL,
                    "Currency" TEXT NOT NULL,
                    "GoogleDriveFileId" TEXT NOT NULL,
                    "FileName" TEXT NOT NULL,
                    "WebViewLink" TEXT NULL,
                    "Category" TEXT NOT NULL,
                    "Notes" TEXT NULL,
                    "Status" INTEGER NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "ArchivedAt" TEXT NULL
                );
                """;
            cmd.ExecuteNonQuery();
        }

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={_dbPath};Pooling=False");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IReceiptFileStorageService));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var storage = Substitute.For<IReceiptFileStorageService>();
                var fileRef = GoogleDriveFileReference.Create("legacy-id", "legacy.pdf", "https://drive.google.com/file/d/legacy-id/view");
                storage.UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<ReceiptUploadMetadata>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(fileRef));
                services.AddSingleton(storage);
            });
        });
        var client = factory.CreateClient();

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent("Legacy Store"), "Merchant");
        form.Add(new StringContent(DateTime.UtcNow.AddHours(-1).ToString("o")), "PurchaseDate");
        form.Add(new StringContent("Groceries"), "Category");
        var file = new ByteArrayContent(Encoding.UTF8.GetBytes("pdf"));
        file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        form.Add(file, "File", "legacy.pdf");

        // Act
        var response = await client.PostAsync("/api/receipts", form);

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, because: body);

        using var verify = new SqliteConnection($"Data Source={_dbPath};Pooling=False");
        verify.Open();
        using var pragma = verify.CreateCommand();
        pragma.CommandText = "PRAGMA table_info(\"Receipts\");";
        var columns = new List<string>();
        using (var reader = pragma.ExecuteReader())
        {
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        columns.Should().NotContain("Amount");
        columns.Should().NotContain("Currency");
        columns.Should().Contain("Merchant");
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        foreach (var path in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
        {
            try { File.Delete(path); } catch { /* best effort cleanup */ }
        }
    }
}
